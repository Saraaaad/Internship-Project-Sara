<#
.SYNOPSIS
    Helper functions for unit test generation and validation.
#>

function Write-StatusMessage {
    param(
        [string]$Message,
        [ValidateSet("Info", "Success", "Warning", "Error")]
        [string]$Status = "Info"
    )
    
    $color = switch ($Status) {
        "Info" { "Cyan" }
        "Success" { "Green" }
        "Warning" { "Yellow" }
        "Error" { "Red" }
    }
    
    Write-Host "[$Status] $Message" -ForegroundColor $color
}

function Test-ControllerExists {
    param([string]$ControllerName)
    
    $controllerPath = Get-ChildItem -Path . -Filter "${ControllerName}.cs" -Recurse | 
        Where-Object { $_.FullName -match "Controllers" } | 
        Select-Object -First 1 -ExpandProperty FullName
    
    return $null -ne $controllerPath
}

function Get-TestProjectPath {
    # Check for common test project locations
    $possiblePaths = @(
        "*.Tests",
        "*.UnitTests",
        "Tests",
        "test"
    )
    
    foreach ($pattern in $possiblePaths) {
        $path = Get-ChildItem -Path . -Filter $pattern -Directory | Select-Object -First 1 -ExpandProperty FullName
        if ($path) {
            return $path
        }
    }
    
    # Check for any .csproj with "Test" in name
    $testProject = Get-ChildItem -Path . -Filter "*.csproj" -Recurse | 
        Where-Object { $_.Name -match "Test|Tests|Unit" } | 
        Select-Object -First 1 -ExpandProperty FullName
    
    if ($testProject) {
        return Split-Path $testProject -Parent
    }
    
    return $null
}

function New-TestProject {
    param(
        [string]$ProjectName = "InternshipProjectSara.Tests",
        [string]$OutputPath = "."
    )
    
    $fullPath = Join-Path $OutputPath $ProjectName
    
    Write-StatusMessage "Creating test project: $ProjectName" -Status Info
    
    # Create xUnit project
    & dotnet new xunit -n $ProjectName -o $fullPath
    
    # Add project reference
    & dotnet add "$fullPath\$ProjectName.csproj" reference "..\InternshipProjectSara.csproj"
    
    # Add required packages
    Push-Location $fullPath
    & dotnet add package Moq
    & dotnet add package FluentAssertions
    & dotnet add package Microsoft.AspNetCore.Mvc.Testing
    & dotnet add package Microsoft.EntityFrameworkCore.InMemory
    Pop-Location
    
    Write-StatusMessage "Test project created successfully" -Status Success
    
    return $fullPath
}

function Get-SnapshotPath {
    param([string]$ControllerName)
    
    return Join-Path ".opencode\snapshots" "${ControllerName}_Tests.json"
}

function Test-SnapshotExists {
    param([string]$ControllerName)
    
    $snapshotPath = Get-SnapshotPath -ControllerName $ControllerName
    return (Test-Path $snapshotPath)
}

function Get-Snapshot {
    param([string]$ControllerName)
    
    $snapshotPath = Get-SnapshotPath -ControllerName $ControllerName
    
    if (Test-Path $snapshotPath) {
        return Get-Content -Path $snapshotPath -Raw | ConvertFrom-Json
    }
    
    return $null
}

function Save-Snapshot {
    param(
        [string]$ControllerName,
        [hashtable]$SnapshotData
    )
    
    $snapshotPath = Get-SnapshotPath -ControllerName $ControllerName
    $snapshotDir = Split-Path $snapshotPath -Parent
    
    if (-not (Test-Path $snapshotDir)) {
        New-Item -ItemType Directory -Path $snapshotDir -Force | Out-Null
    }
    
    $SnapshotData | ConvertTo-Json -Depth 10 | Set-Content -Path $snapshotPath
}

function Test-FileChanged {
    param(
        [string]$FilePath,
        [string]$StoredHash
    )
    
    if (-not (Test-Path $FilePath)) {
        return $true
    }
    
    $currentHash = (Get-FileHash -Path $FilePath -Algorithm SHA256).Hash
    return $currentHash -ne $StoredHash
}

function Get-FileHash256 {
    param([string]$FilePath)
    
    if (Test-Path $FilePath) {
        return (Get-FileHash -Path $FilePath -Algorithm SHA256).Hash
    }
    
    return $null
}

function Compare-TestCoverage {
    param(
        [array]$ControllerActions,
        [array]$TestMethods
    )
    
    $coverage = @{
        Total = $ControllerActions.Count
        Tested = 0
        Missing = @()
    }
    
    foreach ($action in $ControllerActions) {
        $hasTest = $false
        foreach ($test in $TestMethods) {
            if ($test -match "^$($action.Name)_") {
                $hasTest = $true
                break
            }
        }
        
        if ($hasTest) {
            $coverage.Tested++
        } else {
            $coverage.Missing += $action.Name
        }
    }
    
    return $coverage
}

function Format-TestOutput {
    param(
        [string]$TestName,
        [ValidateSet("Pass", "Fail", "Skip")]
        [string]$Status,
        [string]$Message = ""
    )
    
    $icon = switch ($Status) {
        "Pass" { "[PASS]" }
        "Fail" { "[FAIL]" }
        "Skip" { "[SKIP]" }
    }
    
    $color = switch ($Status) {
        "Pass" { "Green" }
        "Fail" { "Red" }
        "Skip" { "Yellow" }
    }
    
    $output = "$icon $TestName"
    if ($Message) {
        $output += " - $Message"
    }
    
    Write-Host $output -ForegroundColor $color
}
