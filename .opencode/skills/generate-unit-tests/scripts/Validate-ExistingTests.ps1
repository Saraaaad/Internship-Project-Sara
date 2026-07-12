<#
.SYNOPSIS
    Validates existing unit tests against current controller implementation.

.DESCRIPTION
    This script checks if existing unit tests are up-to-date with the current
    controller implementation and identifies missing or outdated tests.

.PARAMETER ControllerName
    The name of the controller to validate tests for (e.g., "UserController").

.PARAMETER TestProjectPath
    Path to the test project directory.

.EXAMPLE
    .\Validate-ExistingTests.ps1 -ControllerName "UserController" -TestProjectPath "InternshipProjectSara.Tests"
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$ControllerName,
    
    [Parameter(Mandatory = $false)]
    [string]$TestProjectPath = "InternshipProjectSara.Tests"
)

# Import helper functions
$helperPath = Join-Path $PSScriptRoot "..\templates\TestHelpers.ps1"
if (Test-Path $helperPath) {
    . $helperPath
} else {
    # Define minimal helper if not found
    function Write-StatusMessage {
        param([string]$Message, [string]$Status = "Info")
        Write-Host "[$Status] $Message" -ForegroundColor $(switch ($Status) { "Info" { "Cyan" } "Success" { "Green" } "Warning" { "Yellow" } "Error" { "Red" } default { "White" } })
    }
}

function Get-ControllerActions {
    param([string]$ControllerPath)
    
    $content = Get-Content -Path $ControllerPath -Raw
    $actions = @()
    
    # Extract action methods with HTTP attributes (including those with parameters)
    $pattern = '\[Http(Get|Post|Put|Patch|Delete).*?\]'
    $matches = [regex]::Matches($content, $pattern)
    
    foreach ($match in $matches) {
        $lineNumber = $content.Substring(0, $match.Index).Split("`n").Count
        # Look at the next line for the method signature (attribute is on separate line)
        $lines = Get-Content -Path $ControllerPath
        if ($lineNumber -lt $lines.Count) {
            $line = $lines[$lineNumber]  # Next line (0-indexed, so $lineNumber is next)
        } else {
            $line = ""
        }
        
        # Extract method name from the line after the attribute
        $methodPattern = 'public\s+\S+\s+(\w+)\s*\('
        $methodMatch = [regex]::Match($line, $methodPattern)
        
        if ($methodMatch.Success) {
            $actions += @{
                Name = $methodMatch.Groups[1].Value
                HttpMethod = $match.Groups[1].Value
                LineNumber = $lineNumber + 1  # Method is on next line
            }
        }
    }
    
    return $actions
}

function Get-TestMethods {
    param([string]$TestFilePath)
    
    if (-not (Test-Path $TestFilePath)) {
        return @()
    }
    
    $content = Get-Content -Path $TestFilePath -Raw
    $testMethods = @()
    
    # Extract test methods (methods with [Fact] or [Theory] attribute)
    $pattern = '\[(Fact|Theory)\].*?\n\s*public\s+\S+\s+(\w+)\s*\('
    $matches = [regex]::Matches($content, $pattern, [System.Text.RegularExpressions.RegexOptions]::Singleline)
    
    foreach ($match in $matches) {
        $testMethods += @{
            Name = $match.Groups[2].Value
            Attribute = $match.Groups[1].Value
        }
    }
    
    return $testMethods
}

function Find-MissingTests {
    param(
        [array]$ControllerActions,
        [array]$TestMethods
    )
    
    $missingTests = @()
    
    foreach ($action in $ControllerActions) {
        $methodName = $action.Name
        $authorizeAttribute = $action.AuthorizeAttribute
        
        # Check if there's a test method for this action
        $hasSuccessTest = $false
        $hasExceptionTest = $false
        $hasAuthorizationTest = $false
        
        foreach ($test in $TestMethods) {
            # Match various success test patterns
            if ($test.Name -match "^${methodName}_When\w+_Returns(Ok|CreatedAt|Success|NoContent)") {
                $hasSuccessTest = $true
            }
            # Match various exception test patterns
            if ($test.Name -match "^${methodName}_When(Exception|Error|Thrown)") {
                $hasExceptionTest = $true
            }
            # Match various authorization test patterns
            if ($test.Name -match "^${methodName}_When(Unauthorized|Forbidden|CannotAccess|NotAuthorized)") {
                $hasAuthorizationTest = $true
            }
        }
        
        # Check for missing success test
        if (-not $hasSuccessTest) {
            $missingTests += @{
                Action = $methodName
                HttpMethod = $action.HttpMethod
                Type = "Success"
                Reason = "No success test method found for action '$methodName'"
            }
        }
        
        # Check for missing exception test
        if (-not $hasExceptionTest) {
            $missingTests += @{
                Action = $methodName
                HttpMethod = $action.HttpMethod
                Type = "Exception"
                Reason = "No exception test method found for action '$methodName'"
            }
        }
        
        # Check for missing authorization test (if action has authorize attribute)
        if ($authorizeAttribute -and -not $hasAuthorizationTest) {
            $missingTests += @{
                Action = $methodName
                HttpMethod = $action.HttpMethod
                Type = "Authorization"
                Reason = "No authorization test method found for action '$methodName' with [Authorize] attribute"
            }
        }
    }
    
    return $missingTests
}

function Find-OutdatedTests {
    param(
        [string]$ControllerPath,
        [string]$TestFilePath
    )
    
    if (-not (Test-Path $TestFilePath)) {
        return @()
    }
    
    $outdatedTests = @()
    $controllerContent = Get-Content -Path $ControllerPath -Raw
    $testContent = Get-Content -Path $TestFilePath -Raw
    
    # Check for common outdated patterns
    $outdatedPatterns = @(
        @{
            Pattern = 'ActionResult<UserDto>'
            Message = "Response type 'UserDto' should be 'UserResponseDto'"
        },
        @{
            Pattern = 'StatusCode\(500\)'
            Message = "Direct StatusCode(500) should use ObjectResult with exception message"
        },
        @{
            Pattern = 'Assert\.Equal\(200'
            Message = "Use FluentAssertions: result.Should().BeOfType<OkObjectResult>()"
        }
    )
    
    foreach ($pattern in $outdatedPatterns) {
        if ($testContent -match $pattern.Pattern) {
            $outdatedTests += @{
                Pattern = $pattern.Pattern
                Message = $pattern.Message
            }
        }
    }
    
    return $outdatedTests
}

function Generate-ValidationReport {
    param(
        [string]$ControllerName,
        [array]$MissingTests,
        [array]$OutdatedTests,
        [int]$TotalActions,
        [int]$TestedActions,
        [int]$TotalTestMethods
    )
    
    # Calculate authorization test coverage
    $actionsWithAuth = @()
    $actionsWithAuthTests = @()
    
    foreach ($test in $MissingTests) {
        if ($test.Type -eq "Authorization") {
            $actionsWithAuth += $test.Action
        }
    }
    
    # Count unique missing actions
    $uniqueMissingActions = @($MissingTests | ForEach-Object { $_.Action } | Select-Object -Unique)
    
    $report = @{
        ControllerName = $ControllerName
        GeneratedAt = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
        Summary = @{
            TotalActions = $TotalActions
            TestedActions = $TestedActions
            TotalTestMethods = $TotalTestMethods
            MissingTestsCount = $uniqueMissingActions.Count
            OutdatedTestsCount = $OutdatedTests.Count
            MissingSuccessTests = ($MissingTests | Where-Object { $_.Type -eq "Success" }).Count
            MissingExceptionTests = ($MissingTests | Where-Object { $_.Type -eq "Exception" }).Count
            MissingAuthorizationTests = ($MissingTests | Where-Object { $_.Type -eq "Authorization" }).Count
        }
        MissingTests = $MissingTests
        OutdatedTests = $OutdatedTests
        Recommendations = @()
    }
    
    # Generate recommendations
    if ($MissingTests.Count -gt 0) {
        $successMissing = ($MissingTests | Where-Object { $_.Type -eq "Success" } | ForEach-Object { $_.Action } | Select-Object -Unique)
        $exceptionMissing = ($MissingTests | Where-Object { $_.Type -eq "Exception" } | ForEach-Object { $_.Action } | Select-Object -Unique)
        $authMissing = ($MissingTests | Where-Object { $_.Type -eq "Authorization" } | ForEach-Object { $_.Action } | Select-Object -Unique)
        
        if ($successMissing.Count -gt 0) {
            $report.Recommendations += "Add success test methods for: $($successMissing -join ', ')"
        }
        if ($exceptionMissing.Count -gt 0) {
            $report.Recommendations += "Add exception test methods for: $($exceptionMissing -join ', ')"
        }
        if ($authMissing.Count -gt 0) {
            $report.Recommendations += "Add authorization test methods for: $($authMissing -join ', ')"
        }
    }
    
    if ($OutdatedTests.Count -gt 0) {
        $report.Recommendations += "Update outdated test patterns to use current conventions"
    }
    
    if ($TotalActions -eq $TestedActions -and $OutdatedTests.Count -eq 0) {
        $report.Recommendations += "All actions are tested with current patterns"
    }
    
    return $report
}

# Main execution
try {
    Write-Host "Validating tests for controller: $ControllerName" -ForegroundColor Cyan
    
    # Normalize controller name - add "Controller" suffix if not present
    $searchName = $ControllerName
    if (-not $searchName.EndsWith("Controller")) {
        $searchName = "${searchName}Controller"
    }
    
    # Find controller file
    $controllerPath = Get-ChildItem -Path . -Filter "${searchName}.cs" -Recurse | 
        Where-Object { $_.FullName -match "Controllers" -and $_.FullName -notmatch "Tests" } | 
        Select-Object -First 1 -ExpandProperty FullName
    
    if (-not $controllerPath) {
        Write-Error "Controller file not found: $searchName.cs"
        exit 1
    }
    
    Write-Host "Found controller: $controllerPath" -ForegroundColor Green
    
    # Find test file - use the base name (without Controller suffix)
    $baseName = $ControllerName
    if ($baseName.EndsWith("Controller")) {
        $baseName = $baseName.Substring(0, $baseName.Length - 10)
    }
    $testFilePath = Join-Path (Join-Path $TestProjectPath "Controllers") "${baseName}ControllerTests.cs"
    
    # Get controller actions
    $controllerActions = Get-ControllerActions -ControllerPath $controllerPath
    Write-Host "Found $($controllerActions.Count) actions in controller" -ForegroundColor Yellow
    
    # Get existing test methods
    $testMethods = Get-TestMethods -TestFilePath $testFilePath
    Write-Host "Found $($testMethods.Count) existing test methods" -ForegroundColor Yellow
    
    # Find missing tests
    $missingTests = Find-MissingTests -ControllerActions $controllerActions -TestMethods $testMethods
    Write-Host "Missing tests: $($missingTests.Count)" -ForegroundColor $(if ($missingTests.Count -gt 0) { "Red" } else { "Green" })
    
    # Find outdated tests
    $outdatedTests = Find-OutdatedTests -ControllerPath $controllerPath -TestFilePath $testFilePath
    Write-Host "Outdated tests: $($outdatedTests.Count)" -ForegroundColor $(if ($outdatedTests.Count -gt 0) { "Red" } else { "Green" })
    
    # Generate report
    $report = Generate-ValidationReport -ControllerName $ControllerName -MissingTests $missingTests -OutdatedTests $outdatedTests -TotalActions $controllerActions.Count -TestedActions ($controllerActions.Count - ($missingTests | Where-Object { $_.Type -eq "Success" } | Select-Object -Unique -Property Action).Count) -TotalTestMethods $testMethods.Count
    
    # Output report
    Write-Host "`nValidation Report:" -ForegroundColor Cyan
    Write-Host "==================" -ForegroundColor Cyan
    Write-Host "Total Actions: $($report.Summary.TotalActions)" -ForegroundColor White
    Write-Host "Tested Actions: $($report.Summary.TestedActions)" -ForegroundColor White
    Write-Host "Missing Tests: $($report.Summary.MissingTestsCount)" -ForegroundColor $(if ($report.Summary.MissingTestsCount -gt 0) { "Red" } else { "Green" })
    Write-Host "Outdated Tests: $($report.Summary.OutdatedTestsCount)" -ForegroundColor $(if ($report.Summary.OutdatedTestsCount -gt 0) { "Red" } else { "Green" })
    
    if ($report.MissingTests.Count -gt 0) {
        Write-Host "`nMissing Tests:" -ForegroundColor Red
        foreach ($test in $report.MissingTests) {
            Write-Host "  - $($test.Action) ($($test.HttpMethod)): $($test.Reason)" -ForegroundColor Yellow
        }
    }
    
    if ($report.OutdatedTests.Count -gt 0) {
        Write-Host "`nOutdated Tests:" -ForegroundColor Red
        foreach ($test in $report.OutdatedTests) {
            Write-Host "  - $($test.Message)" -ForegroundColor Yellow
        }
    }
    
    if ($report.Recommendations.Count -gt 0) {
        Write-Host "`nRecommendations:" -ForegroundColor Cyan
        foreach ($rec in $report.Recommendations) {
            Write-Host "  - $rec" -ForegroundColor White
        }
    }
    
    # Save report to JSON
    $reportPath = Join-Path (Join-Path $TestProjectPath "ValidationReports") "${ControllerName}_ValidationReport.json"
    $reportDir = Split-Path $reportPath -Parent
    
    if (-not (Test-Path $reportDir)) {
        New-Item -ItemType Directory -Path $reportDir -Force | Out-Null
    }
    
    $report | ConvertTo-Json -Depth 10 | Set-Content -Path $reportPath
    Write-Host "`nReport saved to: $reportPath" -ForegroundColor Green
    
    # Return status as object for caller
    $result = @{
        MissingTests = $missingTests
        OutdatedTests = $outdatedTests
        Report = $report
        IsValid = ($missingTests.Count -eq 0 -and $outdatedTests.Count -eq 0)
    }
    return $result
    
} catch {
    Write-Error "Error validating tests: $_"
    return @{ IsValid = $false; Error = $_.Exception.Message }
}
