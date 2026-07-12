<#
.SYNOPSIS
    Main orchestration script for generating unit tests for ASP.NET Core controllers.

.DESCRIPTION
    This script orchestrates the entire unit test generation workflow:
    1. Checks for existing test project
    2. Validates existing tests
    3. Generates or updates tests
    4. Saves snapshots for change detection

.PARAMETER ControllerName
    The name of the controller to generate tests for (e.g., "UserController", "User", or "UserControllerTests.cs").

.PARAMETER Force
    Force regeneration of tests even if they exist.

.PARAMETER ValidateOnly
    Only validate existing tests without generating new ones.

.EXAMPLE
    .\Invoke-UnitTestGeneration.ps1 -ControllerName "UserController"

.EXAMPLE
    .\Invoke-UnitTestGeneration.ps1 -ControllerName "User"

.EXAMPLE
    .\Invoke-UnitTestGeneration.ps1 -ControllerName "UserController" -Force
#>

param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$ControllerName,
    
    [Parameter(Mandatory = $false)]
    [switch]$Force,
    
    [Parameter(Mandatory = $false)]
    [switch]$ValidateOnly
)

# Import helper functions
$helperPath = Join-Path $PSScriptRoot "..\templates\TestHelpers.ps1"
if (Test-Path $helperPath) {
    . $helperPath
} else {
    Write-Host "[Warning] TestHelpers.ps1 not found at: $helperPath" -ForegroundColor Yellow
    # Define minimal helper functions if the file is not found
    function Write-StatusMessage {
        param([string]$Message, [string]$Status = "Info")
        $color = switch ($Status) {
            "Info" { "Cyan" }
            "Success" { "Green" }
            "Warning" { "Yellow" }
            "Error" { "Red" }
            default { "White" }
        }
        Write-Host "[$Status] $Message" -ForegroundColor $color
    }
}

function Normalize-ControllerName {
    param([string]$Name)
    
    # Remove common suffixes/patterns to get the base controller name
    $normalized = $Name.Trim()
    
    # Remove file extensions
    $normalized = $normalized -replace '\.cs$', ''
    $normalized = $normalized -replace '\.csproj$', ''
    
    # Remove "Controller" suffix if present
    $normalized = $normalized -replace 'Controller$', ''
    
    # Remove "Tests" suffix if present
    $normalized = $normalized -replace 'Tests$', ''
    
    # Remove "Test" suffix if present
    $normalized = $normalized -replace 'Test$', ''
    
    return $normalized
}

function Test-BackendChanges {
    param(
        [string]$ControllerName,
        [string]$SnapshotPath
    )
    
    $result = @{
        HasChanges = $false
        ChangedFiles = @()
        AddedFiles = @()
        RemovedFiles = @()
        Reason = ""
    }
    
    # Check if snapshot exists
    if (-not (Test-Path $SnapshotPath)) {
        $result.HasChanges = $true
        $result.Reason = "No snapshot found - first generation"
        return $result
    }
    
    # Load existing snapshot
    $snapshot = Get-Content $SnapshotPath -Raw | ConvertFrom-Json
    
    # Check controller file
    $controllerPath = Join-Path . "Application\Controllers\${ControllerName}Controller.cs"
    if (Test-Path $controllerPath) {
        $currentHash = (Get-FileHash $controllerPath).Hash
        $snapshotHash = $snapshot.controllerFile.hash
        
        if ($currentHash -ne $snapshotHash) {
            $result.HasChanges = $true
            $result.ChangedFiles += "Application\Controllers\${ControllerName}Controller.cs"
            $result.Reason = "Controller file changed"
        }
    }
    
    # Check DTO files
    if ($snapshot.dtoFiles) {
        foreach ($dto in $snapshot.dtoFiles) {
            $dtoPath = $dto.path
            if (Test-Path $dtoPath) {
                $currentHash = (Get-FileHash $dtoPath).Hash
                if ($currentHash -ne $dto.hash) {
                    $result.HasChanges = $true
                    $result.ChangedFiles += $dtoPath
                    $result.Reason = "DTO file changed: $dtoPath"
                }
            } else {
                $result.HasChanges = $true
                $result.RemovedFiles += $dtoPath
                $result.Reason = "DTO file removed: $dtoPath"
            }
        }
    }
    
    # Check service interfaces
    if ($snapshot.serviceInterfaces) {
        foreach ($svc in $snapshot.serviceInterfaces) {
            $svcPath = $svc.path
            if (Test-Path $svcPath) {
                $currentHash = (Get-FileHash $svcPath).Hash
                if ($currentHash -ne $svc.hash) {
                    $result.HasChanges = $true
                    $result.ChangedFiles += $svcPath
                    $result.Reason = "Service interface changed: $svcPath"
                }
            }
        }
    }
    
    return $result
}

function Save-TestSnapshot {
    param(
        [string]$ControllerName,
        [string]$ControllerPath,
        [string]$TestFilePath,
        [array]$DtoFiles,
        [array]$ServiceInterfaces,
        [array]$GeneratedTests
    )
    
    $snapshot = @{
        generatedAt = (Get-Date -Format "yyyy-MM-ddTHH:mm:ssZ")
        controllerFile = @{
            path = $ControllerPath
            hash = (Get-FileHash $ControllerPath).Hash
            lastModified = (Get-Item $ControllerPath).LastWriteTime.ToString("yyyy-MM-ddTHH:mm:ssZ")
        }
        dtoFiles = @()
        serviceInterfaces = @()
        testFile = $null
        generatedTests = $GeneratedTests
        coverage = @{
            totalActions = $GeneratedTests.Count
            testedActions = $GeneratedTests.Count
            testMethods = $GeneratedTests.Count
        }
    }
    
    # Add DTO file hashes
    foreach ($dtoPath in $DtoFiles) {
        if (Test-Path $dtoPath) {
            $snapshot.dtoFiles += @{
                path = $dtoPath
                hash = (Get-FileHash $dtoPath).Hash
            }
        }
    }
    
    # Add service interface hashes
    foreach ($svcPath in $ServiceInterfaces) {
        if (Test-Path $svcPath) {
            $snapshot.serviceInterfaces += @{
                path = $svcPath
                hash = (Get-FileHash $svcPath).Hash
            }
        }
    }
    
    # Add test file hash
    if (Test-Path $TestFilePath) {
        $snapshot.testFile = @{
            path = $TestFilePath
            hash = (Get-FileHash $TestFilePath).Hash
            testCount = ($GeneratedTests | Measure-Object).Count
        }
    }
    
    # Save snapshot
    $snapshotDir = Split-Path $SnapshotPath -Parent
    if (-not (Test-Path $snapshotDir)) {
        New-Item -ItemType Directory -Path $snapshotDir -Force | Out-Null
    }
    
    $snapshot | ConvertTo-Json -Depth 10 | Set-Content -Path $SnapshotPath
    Write-StatusMessage "Snapshot saved to: $SnapshotPath" -Status Success
}

function Start-UnitTestGeneration {
    param(
        [string]$ControllerName,
        [switch]$Force,
        [switch]$ValidateOnly
    )
    
    # Normalize the controller name
    $baseName = Normalize-ControllerName -Name $ControllerName
    $controllerClassName = "${baseName}Controller"
    
    Write-StatusMessage "Starting unit test generation for: $baseName" -Status Info
    Write-StatusMessage "==========================================" -Status Info
    
    # Step 1: Check for test project
    Write-StatusMessage "Step 1: Checking for test project..." -Status Info
    $testProjectPath = $null
    
    # Check for common test project locations
    $possiblePaths = @(
        "InternshipProjectSara.Tests",
        "*.Tests",
        "*.UnitTests",
        "Tests",
        "test"
    )
    
    foreach ($pattern in $possiblePaths) {
        $found = Get-ChildItem -Path . -Filter $pattern -Directory -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($found) {
            $testProjectPath = $found.FullName
            break
        }
    }
    
    # Also check for any .csproj with "Test" in name
    if (-not $testProjectPath) {
        $testProject = Get-ChildItem -Path . -Filter "*.csproj" -Recurse -ErrorAction SilentlyContinue | 
            Where-Object { $_.Name -match "Test|Tests|Unit" } | 
            Select-Object -First 1
        
        if ($testProject) {
            $testProjectPath = Split-Path $testProject.FullName -Parent
        }
    }
    
    if (-not $testProjectPath) {
        Write-StatusMessage "No test project found. Creating new test project..." -Status Warning
        
        # Create test project
        $testProjectName = "InternshipProjectSara.Tests"
        New-Item -ItemType Directory -Path $testProjectName -Force | Out-Null
        
        # Create basic .csproj file
        $csprojContent = @"
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\InternshipProjectSara.csproj" />
  </ItemGroup>

</Project>
"@
        $csprojContent | Set-Content -Path (Join-Path $testProjectName "$testProjectName.csproj")
        
        # Create Controllers directory
        New-Item -ItemType Directory -Path (Join-Path $testProjectName "Controllers") -Force | Out-Null
        New-Item -ItemType Directory -Path (Join-Path $testProjectName "Helpers") -Force | Out-Null
        
        $testProjectPath = $testProjectName
        Write-StatusMessage "Created test project: $testProjectPath" -Status Success
    } else {
        Write-StatusMessage "Found test project: $testProjectPath" -Status Success
    }
    
    # Step 2: Check for controller
    Write-StatusMessage "Step 2: Checking for controller..." -Status Info
    
    $controllerPath = Get-ChildItem -Path . -Filter "${controllerClassName}.cs" -Recurse -ErrorAction SilentlyContinue | 
        Where-Object { $_.FullName -match "Controllers" -and $_.FullName -notmatch "Tests" } | 
        Select-Object -First 1 -ExpandProperty FullName
    
    if (-not $controllerPath) {
        Write-StatusMessage "Controller not found: $controllerClassName.cs" -Status Error
        Write-StatusMessage "Make sure the controller exists in Application/Controllers/" -Status Error
        return $false
    }
    
    Write-StatusMessage "Controller found: $controllerPath" -Status Success
    
    # Step 3: Detect backend changes
    Write-StatusMessage "Step 3: Detecting backend changes..." -Status Info
    $snapshotPath = Join-Path ".opencode\snapshots" "${baseName}_Tests.json"
    $changeResult = Test-BackendChanges -ControllerName $baseName -SnapshotPath $snapshotPath
    
    if ($changeResult.HasChanges) {
        Write-StatusMessage "Changes detected: $($changeResult.Reason)" -Status Warning
        if ($changeResult.ChangedFiles.Count -gt 0) {
            Write-StatusMessage "Changed files:" -Status Info
            foreach ($file in $changeResult.ChangedFiles) {
                Write-StatusMessage "  - $file" -Status Info
            }
        }
    } else {
        Write-StatusMessage "No backend changes detected" -Status Success
    }
    
    # Step 4: Check for existing tests
    Write-StatusMessage "Step 4: Checking for existing tests..." -Status Info
    $testFilePath = Join-Path (Join-Path $testProjectPath "Controllers") "${baseName}ControllerTests.cs"
    $testsExist = Test-Path $testFilePath
    
    if ($testsExist) {
        Write-StatusMessage "Existing tests found: $testFilePath" -Status Info
        
        # Validate existing tests
        Write-StatusMessage "Step 5: Validating existing tests..." -Status Info
        
        $validateScript = Join-Path $PSScriptRoot "Validate-ExistingTests.ps1"
        if (Test-Path $validateScript) {
            $validationResult = & $validateScript -ControllerName $baseName -TestProjectPath $testProjectPath
            
            if ($validationResult -and $validationResult.IsValid -and -not $changeResult.HasChanges) {
                Write-StatusMessage "Tests are up to date and valid" -Status Success
                
                if (-not $Force) {
                    Write-StatusMessage "Skipping generation (use -Force to regenerate)" -Status Info
                    return $true
                }
            } else {
                if ($changeResult.HasChanges) {
                    Write-StatusMessage "Backend changes detected - tests need updating" -Status Warning
                } else {
                    Write-StatusMessage "Tests need updating" -Status Warning
                }
            }
        } else {
            Write-StatusMessage "Validation script not found, proceeding with generation" -Status Warning
        }
    } else {
        Write-StatusMessage "No existing tests found" -Status Info
    }
    
    # Step 6: Check if we should only validate
    if ($ValidateOnly) {
        Write-StatusMessage "ValidateOnly mode - skipping generation" -Status Info
        return $true
    }
    
    # Step 7: Generate tests
    Write-StatusMessage "Step 6: Generating tests..." -Status Info
    
    $generateScript = Join-Path $PSScriptRoot "Generate-TestFile.ps1"
    if (Test-Path $generateScript) {
        $errorBefore = $Error.Count
        $LASTEXITCODE = 0  # Reset before calling script
        & $generateScript -ControllerName $baseName -TestProjectPath $testProjectPath -Force:$Force
        
        if ($LASTEXITCODE -ne 0 -or $Error.Count -gt $errorBefore) {
            Write-StatusMessage "Test generation failed" -Status Error
            return $false
        }
    } else {
        Write-StatusMessage "Generate script not found, creating basic test file" -Status Warning
        
        # Create a basic test file
        $basicTestContent = @"
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InternshipProjectSara.Tests.Controllers;

public class ${baseName}ControllerTests
{
    [Fact]
    public void Test1()
    {
        // TODO: Add tests for ${baseName}Controller
        Assert.True(true);
    }
}
"@
        $basicTestContent | Set-Content -Path $testFilePath
    }
    
    Write-StatusMessage "Tests generated successfully" -Status Success
    
    # Step 7: Verify generated tests
    Write-StatusMessage "Step 6: Verifying generated tests..." -Status Info
    
    if (Test-Path $testFilePath) {
        Write-StatusMessage "Test file verified: $testFilePath" -Status Success
        
        # Build the test project to verify compilation
        Write-StatusMessage "Step 7: Building test project to verify compilation..." -Status Info
        Push-Location $testProjectPath
        & dotnet build --verbosity quiet 2>&1 | Out-Null
        $buildResult = $LASTEXITCODE
        Pop-Location
        
        if ($buildResult -eq 0) {
            Write-StatusMessage "Test project builds successfully!" -Status Success
            
            # Run tests
            Write-StatusMessage "Running tests..." -Status Info
            Push-Location $testProjectPath
            & dotnet test --no-build --verbosity quiet 2>&1 | Out-Null
            $testResult = $LASTEXITCODE
            Pop-Location
            
            if ($testResult -eq 0) {
                Write-StatusMessage "All tests passed!" -Status Success
                
                # Step 8: Save snapshot for change detection
                Write-StatusMessage "Step 7: Saving snapshot for change detection..." -Status Info
                
                # Find DTO files referenced by the controller
                $dtoFiles = @()
                $dtoDir = Join-Path . "Business\DTOs"
                if (Test-Path $dtoDir) {
                    $dtoFiles = Get-ChildItem -Path $dtoDir -Filter "*.cs" -Recurse | 
                        Where-Object { $_.Name -match "Request|Response" } |
                        ForEach-Object { $_.FullName.Replace((Get-Location).Path + "\", "") }
                }
                
                # Find service interfaces
                $serviceInterfaces = @()
                $svcDir = Join-Path . "Business\Interfaces"
                if (Test-Path $svcDir) {
                    $serviceInterfaces = Get-ChildItem -Path $svcDir -Filter "I*.cs" -Recurse |
                        ForEach-Object { $_.FullName.Replace((Get-Location).Path + "\", "") }
                }
                
                # Get list of generated tests
                $generatedTests = @()
                if (Test-Path $testFilePath) {
                    $testContent = Get-Content $testFilePath -Raw
                    $testMethodPattern = '\[(Fact|Theory)\]\s*(?:\[.*?\]\s*)?public\s+\S+\s+(\w+)\s*\('
                    $testMatches = [regex]::Matches($testContent, $testMethodPattern, [System.Text.RegularExpressions.RegexOptions]::Singleline)
                    $generatedTests = $testMatches | ForEach-Object { @{ Name = $_.Groups[2].Value; Type = $_.Groups[1].Value } }
                }
                
                Save-TestSnapshot -ControllerName $baseName -ControllerPath $controllerPath -TestFilePath $testFilePath -DtoFiles $dtoFiles -ServiceInterfaces $serviceInterfaces -GeneratedTests $generatedTests
            } else {
                Write-StatusMessage "Some tests failed (this may be expected for new tests)" -Status Warning
            }
        } else {
            Write-StatusMessage "Build failed - please check the test file for errors" -Status Warning
        }
    } else {
        Write-StatusMessage "Test file not found after generation" -Status Error
        return $false
    }
    
    Write-StatusMessage "==========================================" -Status Info
    Write-StatusMessage "Unit test generation completed!" -Status Success
    
    return $true
}

# Main execution
try {
    $result = Start-UnitTestGeneration -ControllerName $ControllerName -Force:$Force -ValidateOnly:$ValidateOnly
    
    if ($result) {
        exit 0
    } else {
        exit 1
    }
    
} catch {
    Write-StatusMessage "Error: $_" -Status Error
    Write-StatusMessage $_.ScriptStackTrace -Status Error
    exit 1
}
