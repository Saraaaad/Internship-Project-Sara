# Generate Unit Tests for Controller Skill

This skill generates comprehensive unit tests for ASP.NET Core controllers with proper mocking, assertions, and test coverage.

## Features

- **Automatic Test Project Detection**: Checks for existing test project, creates one if needed
- **Test Validation**: Validates existing tests against current controller implementation
- **Smart Regeneration**: Only regenerates tests when controller has changed
- **Snapshot Management**: Stores test generation metadata for change detection
- **Comprehensive Coverage**: Generates tests for success, error, and authorization scenarios

## Usage

### Via OpenCode Agent

The skill is automatically invoked when you request unit test generation for a controller:

```
Generate unit tests for UserController
Create tests for TaskController
```

### Via PowerShell Scripts

#### Generate Tests for a Controller

The script accepts the controller name in various formats:
- `"UserController"` - Full controller name
- `"User"` - Base name without "Controller" suffix
- `"UserControllerTests.cs"` - File name (suffixes are automatically removed)

```powershell
# All of these work the same way:
.\scripts\Invoke-UnitTestGeneration.ps1 -ControllerName "UserController"
.\scripts\Invoke-UnitTestGeneration.ps1 -ControllerName "User"
.\scripts\Invoke-UnitTestGeneration.ps1 "UserController"
```

#### Force Regeneration

```powershell
.\scripts\Invoke-UnitTestGeneration.ps1 -ControllerName "UserController" -Force
```

#### Validate Only (No Generation)

```powershell
.\scripts\Invoke-UnitTestGeneration.ps1 -ControllerName "UserController" -ValidateOnly
```

### Direct Script Usage

#### Generate Test File

```powershell
.\scripts\Generate-TestFile.ps1 -ControllerName "UserController" -Force
```

#### Validate Existing Tests

```powershell
.\scripts\Validate-ExistingTests.ps1 -ControllerName "UserController"
```

## Workflow

1. **Check Test Project**: Verifies test project exists, creates if not
2. **Parse Controller**: Extracts actions, dependencies, and DTOs
3. **Validate Existing Tests**: Checks for missing or outdated tests
4. **Generate/Update Tests**: Creates new or updates existing tests
5. **Save Snapshot**: Stores metadata for future change detection

## Generated Test Structure

```
InternshipProjectSara.Tests/
├── Controllers/
│   ├── UserControllerTests.cs
│   ├── TaskControllerTests.cs
│   └── ...
├── Helpers/
│   ├── TestBase.cs
│   ├── MockFactory.cs
│   └── TestDataGenerator.cs
└── ValidationReports/
    └── UserController_ValidationReport.json
```

## Test Categories

For each controller action, the skill generates:

1. **Success Tests**: Verify correct return type and data
2. **Exception Tests**: Verify error handling returns 500
3. **Authorization Tests**: Verify role-based access control

## Dependencies

- **Test Framework**: xUnit
- **Mocking Framework**: Moq
- **Assertion Framework**: FluentAssertions
- **Additional**: Microsoft.AspNetCore.Mvc.Testing, Microsoft.EntityFrameworkCore.InMemory

## Snapshot Management

After generating tests, a snapshot is saved to:
```
.opencode/snapshots/<ControllerName>_Tests.json
```

This snapshot is used to:
- Detect if controller has changed since last generation
- Track which tests were generated
- Store test coverage metadata

## Validation Reports

When validating existing tests, a report is saved to:
```
InternshipProjectSara.Tests/ValidationReports/<ControllerName>_ValidationReport.json
```

The report includes:
- Total actions in controller
- Number of tested actions
- Missing tests
- Outdated test patterns
- Recommendations

## Customization

### Adding Custom Test Helpers

Edit the template files in the `templates/` directory:

- `TestBase.cs.template`: Base class for all tests
- `MockFactory.cs.template`: Mock object factory

### Modifying Test Patterns

Edit the `Generate-TestFile.ps1` script to customize:
- Test naming conventions
- Mock setup patterns
- Assertion patterns

## Troubleshooting

### "A positional parameter cannot be found" Error

This error occurs when the script receives unexpected arguments. The script now handles this automatically by:
- Accepting controller names in various formats (`"UserController"`, `"User"`, `"UserControllerTests.cs"`)
- Automatically removing suffixes like "Controller", "Tests", or file extensions
- Using named parameters instead of positional ones

Example of correct usage:
```powershell
.\scripts\Invoke-UnitTestGeneration.ps1 -ControllerName "UserController"
.\scripts\Invoke-UnitTestGeneration.ps1 -ControllerName "User"
```

### Tests Not Generating

1. Verify controller exists in `Application/Controllers/`
2. Check test project path is correct
3. Ensure required NuGet packages are installed

### Tests Failing

1. Check mock setups match actual service interfaces
2. Verify DTO properties match controller expectations
3. Review authorization service mock behavior

### Validation Errors

1. Check controller has changed since last generation
2. Review validation report for specific issues
3. Use `-Force` flag to regenerate tests

### Build Errors

If you encounter build errors like:
- `The type or namespace name 'FluentAssertions' could not be found`
- `The type or namespace name 'FactAttribute' could not be found`

Run these commands to restore packages and rebuild:
```powershell
dotnet restore InternshipProjectSara.Tests/InternshipProjectSara.Tests.csproj
dotnet build InternshipProjectSara.Tests/InternshipProjectSara.Tests.csproj
```
