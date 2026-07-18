---
name: generate-unit-tests-for-controller
description: Generate comprehensive unit tests for ASP.NET Core controllers. Checks for existing test project, validates existing tests, and generates/updates tests with proper mocking, assertions, and coverage. Use ONLY when generating unit tests for controllers.
---

# Generate Unit Tests for Controller

Generate comprehensive unit tests for ASP.NET Core controllers with proper mocking, assertions, and test coverage.

## Workflow Overview

1. **Check/Test Project** → Verify test project exists, create if needed
2. **Parse Controller** → Extract testable actions, dependencies, and DTOs
3. **Analyze Existing Tests** → Check for existing tests and validate them
4. **Generate/Update Tests** → Create new or update existing tests
5. **Save Snapshot** → Store test generation metadata for future reference

## Phase 1: Test Project Setup

### 1.1 Check for Existing Test Project

Look for test project in these locations (in order):
1. `*.Tests/` or `*.UnitTests/` directory at project root
2. `Tests/` directory at project root
3. `test/` directory at project root
4. Any `.csproj` file containing "Test" in its name

```bash
# Search for test projects
Get-ChildItem -Path . -Filter "*.csproj" -Recurse | Where-Object { $_.Name -match "Test|Tests|Unit" }
```

### 1.2 Create Test Project (if not exists)

If no test project found, create one:

```bash
# Create test project directory
New-Item -ItemType Directory -Path "InternshipProjectSara.Tests" -Force

# Create xUnit test project
dotnet new xunit -n InternshipProjectSara.Tests -o InternshipProjectSara.Tests

# Add project reference
dotnet add InternshipProjectSara.Tests/InternshipProjectSara.Tests.csproj reference InternshipProjectSara.csproj

# Add required packages
cd InternshipProjectSara.Tests
dotnet add package Moq
dotnet add package FluentAssertions
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package Microsoft.EntityFrameworkCore.InMemory
cd ..
```

### 1.3 Test Project Structure

Create organized test structure:
```
InternshipProjectSara.Tests/
├── Controllers/
│   ├── UserControllerTests.cs
│   ├── TaskControllerTests.cs
│   └── ...
├── Services/
│   ├── UserServiceTests.cs
│   └── ...
├── Helpers/
│   ├── TestBase.cs
│   ├── MockFactory.cs
│   └── TestDataGenerator.cs
└── InternshipProjectSara.Tests.csproj
```

## Phase 2: Parse Controller for Testing

### 2.1 Extract Testable Information

From the controller metadata, extract:

```json
{
  "controllerName": "UserController",
  "dependencies": [
    { "name": "uService", "type": "IUserService", "methods": ["GetAll", "GetById", "Create", ...] },
    { "name": "authzService", "type": "IAuthorizationService", "methods": ["CanAccessUserData", "GetCurrentUserId", ...] }
  ],
  "actions": [
    {
      "name": "GetAll",
      "httpMethod": "GET",
      "route": "api/users",
      "authorizeAttribute": "Roles = \"Admin,HR\"",
      "parameters": [],
      "responseType": "ActionResult<List<UserResponseDto>>",
      "callsService": "uService.GetAll()",
      "throwsExceptions": true
    }
  ]
}
```

### 2.2 Identify Test Scenarios

For each action, identify test scenarios:

| Action | Test Scenarios |
|--------|----------------|
| `GetAll` | Success, Exception, Unauthorized, Forbidden (wrong role) |
| `GetById` | Success, Not Found, Exception, Unauthorized, Forbidden (can't access) |
| `Create` | Success, Validation Error, Exception, Unauthorized, Forbidden |
| `Update` | Success, Not Found, Exception, Unauthorized, Forbidden |
| `Delete` | Success, Not Found, Exception, Unauthorized, Forbidden |
| `UpdateSalary` | Success, Not Found, Exception, Unauthorized, Forbidden |
| `GetByDepartmentId` | Success, Empty List, Exception, Unauthorized, Forbidden |
| `GetByEmail` | Success, Not Found, Exception, Unauthorized, Forbidden |
| `GetByUsername` | Success, Not Found, Exception, Unauthorized, Forbidden |
| `GetProfile` | Success, Exception, Unauthorized |
| `ChangePassword` | Success, Invalid Password, Exception, Unauthorized |

## Phase 3: Analyze Existing Tests

### 3.1 Check for Existing Test Files

```bash
# Find existing test files for this controller
$controllerName = "UserController"  # From parsed metadata
$testFiles = Get-ChildItem -Path "InternshipProjectSara.Tests" -Filter "*${controllerName}Tests.cs" -Recurse
```

### 3.2 Validate Existing Tests

If test file exists, validate against current controller:

```csharp
// Validation checklist:
// 1. All controller actions have corresponding test methods
// 2. All dependencies are properly mocked
// 3. Test assertions match current response types
// 4. Authorization attributes are tested
// 5. Exception handling is tested
// 6. Test naming convention is consistent
```

### 3.3 Generate Validation Report

```json
{
  "existingTests": true,
  "testFile": "InternshipProjectSara.Tests/Controllers/UserControllerTests.cs",
  "coverage": {
    "totalActions": 11,
    "testedActions": 8,
    "missingTests": ["GetProfile", "ChangePassword", "UpdateSalary"]
  },
  "issues": [
    {
      "type": "missing_test",
      "action": "GetProfile",
      "description": "No test method for GetProfile action"
    },
    {
      "type": "outdated_assertion",
      "action": "GetById",
      "description": "Response type changed from UserDto to UserResponseDto"
    }
  ],
  "needsUpdate": true
}
```

## Phase 4: Generate/Update Tests

### 4.1 Test File Template

```csharp
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InternshipProjectSara.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IAuthorizationService> _authzServiceMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _authzServiceMock = new Mock<IAuthorizationService>();
        _controller = new UserController(_userServiceMock.Object, _authzServiceMock.Object);
    }

    // Test methods here
}
```

### 4.2 Test Method Patterns

#### Success Test Pattern
```csharp
[Fact]
public async Task GetAll_WhenAuthorized_ReturnsOkWithUsers()
{
    // Arrange
    var users = new List<UserResponseDto>
    {
        new() { Id = 1, Name = "Test User" }
    };
    _userServiceMock.Setup(x => x.GetAll()).Returns(users);

    // Act
    var result = _controller.GetAll();

    // Assert
    var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
    var returnedUsers = okResult.Value.Should().BeAssignableTo<List<UserResponseDto>>().Subject;
    returnedUsers.Should().HaveCount(1);
    returnedUsers[0].Name.Should().Be("Test User");
}
```

#### Exception Test Pattern
```csharp
[Fact]
public void GetAll_WhenExceptionThrown_ReturnsStatusCode500()
{
    // Arrange
    _userServiceMock.Setup(x => x.GetAll())
        .Throws(new Exception("Database error"));

    // Act
    var result = _controller.GetAll();

    // Assert
    var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
    statusCodeResult.StatusCode.Should().Be(500);
    statusCodeResult.Value.Should().Be("Database error");
}
```

#### Authorization Test Pattern
```csharp
[Fact]
public void GetById_WhenCannotAccessUserData_ReturnsForbid()
{
    // Arrange
    var user = new UserResponseDto { Id = 1, EmployeeId = 99 };
    _userServiceMock.Setup(x => x.GetById(1)).Returns(user);
    _authzServiceMock.Setup(x => x.CanAccessUserData(99)).Returns(false);

    // Act
    var result = _controller.GetById(1);

    // Assert
    result.Result.Should().BeOfType<ForbidResult>();
}
```

#### Created Result Test Pattern
```csharp
[Fact]
public void Create_WhenValidDto_ReturnsCreatedAtAction()
{
    // Arrange
    var dto = new UserRequestDto { Name = "New User" };
    var createdUser = new UserResponseDto { Id = 1, Name = "New User" };
    _userServiceMock.Setup(x => x.Create(dto)).Returns(createdUser);

    // Act
    var result = _controller.Create(dto);

    // Assert
    var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
    createdResult.ActionName.Should().Be(nameof(UserController.GetById));
    createdResult.RouteValues["id"].Should().Be(1);
    var returnedUser = createdResult.Value.Should().BeOfType<UserResponseDto>().Subject;
    returnedUser.Name.Should().Be("New User");
}
```

### 4.3 Test Coverage Requirements

Each controller action MUST have these test categories:

1. **Happy Path Tests**
   - Successful execution with valid data
   - Correct return type and status code
   - Proper data transformation

2. **Authorization Tests**
   - Unauthorized access (no token)
   - Forbidden access (wrong role)
   - Authorization service interactions

3. **Error Handling Tests**
   - Exception handling returns 500
   - Service exceptions are properly caught
   - Error messages are preserved

4. **Edge Case Tests**
   - Empty collections
   - Null values where applicable
   - Invalid input validation

5. **Integration Tests** (optional)
   - Full request/response cycle
   - Database interactions (if applicable)

### 4.4 Mock Setup Helper

Create helper class for common mock setups:

```csharp
// Helpers/MockFactory.cs
public static class MockFactory
{
    public static Mock<IUserService> CreateUserServiceMock()
    {
        var mock = new Mock<IUserService>();
        // Default setups
        mock.Setup(x => x.GetAll()).Returns(new List<UserResponseDto>());
        return mock;
    }

    public static Mock<IAuthorizationService> CreateAuthorizationServiceMock(bool isAdmin = false)
    {
        var mock = new Mock<IAuthorizationService>();
        mock.Setup(x => x.IsAdminOrHR()).Returns(isAdmin);
        mock.Setup(x => x.CanAccessUserData(It.IsAny<int>())).Returns(true);
        return mock;
    }
}
```

## Phase 5: Change Detection & Auto-Update

### 5.1 Detect Backend Changes

Before generating or updating tests, detect changes to:

1. **Controller Files** - Check for new/modified actions, parameters, attributes
2. **DTO Files** - Check for property changes, new/removed properties
3. **Service Interfaces** - Check for new/modified methods
4. **Enum Files** - Check for value changes

```powershell
# Load change detection skill
. ".opencode\skills\detect-backend-changes\SKILL.md"

# Compute file hashes
$files = @(
    "Application/Controllers/${ControllerName}Controller.cs",
    # Add referenced DTOs dynamically
)

# Compare with stored snapshot
$snapshotPath = ".opencode\snapshots\${ControllerName}_Tests.json"
if (Test-Path $snapshotPath) {
    $snapshot = Get-Content $snapshotPath | ConvertFrom-Json
    # Compare hashes...
}
```

### 5.2 Detect Test File Changes

Track changes to existing test files:

```json
{
  "testFile": "InternshipProjectSara.Tests/Controllers/UserControllerTests.cs",
  "lastModified": "2026-07-12T10:00:00Z",
  "hash": "sha256hash",
  "testCount": 25,
  "methods": ["GetAll_WhenAuthorized_ReturnsOkWithUsers", ...]
}
```

### 5.3 Auto-Update Workflow

When changes are detected:

```
┌─────────────────────────────────────────────────────────────┐
│                    CHANGE DETECTION                         │
├─────────────────────────────────────────────────────────────┤
│ 1. Controller Changed?                                      │
│    ├─ Yes → Regenerate ALL tests                            │
│    └─ No → Continue                                         │
│                                                             │
│ 2. DTO Changed?                                             │
│    ├─ Yes → Update test data and assertions                 │
│    └─ No → Continue                                         │
│                                                             │
│ 3. Service Interface Changed?                               │
│    ├─ Yes → Add new mock setups, update existing            │
│    └─ No → Continue                                         │
│                                                             │
│ 4. Test File Manually Modified?                             │
│    ├─ Yes → Validate modifications, merge if valid          │
│    └─ No → Continue                                         │
│                                                             │
│ 5. No Changes Detected?                                     │
│    └─ Skip generation, report "Tests are up to date"        │
└─────────────────────────────────────────────────────────────┘
```

### 5.4 Merge Strategy for Manual Edits

When tests are manually modified and backend changes are detected:

1. Parse existing test methods
2. Identify which tests are affected by backend changes
3. Update affected tests while preserving manual improvements
4. Add new tests for new actions
5. Remove tests for deleted actions

```powershell
function Merge-TestFiles {
    param(
        [string]$ExistingTestPath,
        [string]$GeneratedTestPath
    )
    
    # Parse existing tests
    $existingTests = Get-TestMethods -TestFilePath $ExistingTestPath
    
    # Parse generated tests
    $generatedTests = Get-TestMethods -TestFilePath $GeneratedTestPath
    
    # Merge: keep existing test logic, update assertions and mock setups
    # Preserve manually added helper methods
    # Keep custom test data generators
}
```

## Phase 6: Save Snapshot (Updated)

### 6.1 Comprehensive Snapshot Format

Save detailed snapshot including all source file hashes:

```json
{
  "generatedAt": "2026-07-12T10:00:00Z",
  "controllerFile": {
    "path": "Application/Controllers/UserController.cs",
    "hash": "sha256hash",
    "lastModified": "2026-07-12T09:00:00Z"
  },
  "dtoFiles": [
    {
      "path": "Business/DTOs/Request/UserRequestDto.cs",
      "hash": "sha256hash"
    },
    {
      "path": "Business/DTOs/Response/UserResponseDto.cs",
      "hash": "sha256hash"
    }
  ],
  "serviceInterfaces": [
    {
      "path": "Business/Interfaces/IUserService.cs",
      "hash": "sha256hash"
    }
  ],
  "testFile": {
    "path": "InternshipProjectSara.Tests/Controllers/UserControllerTests.cs",
    "hash": "sha256hash",
    "testCount": 25
  },
  "generatedTests": [...],
  "coverage": {...}
}
```

### 6.2 Update Detection Logic

On subsequent runs:

```powershell
function Test-NeedsUpdate {
    param([string]$ControllerName)
    
    $snapshotPath = ".opencode\snapshots\${ControllerName}_Tests.json"
    
    if (-not (Test-Path $snapshotPath)) {
        return @{ NeedsUpdate = $true; Reason = "No snapshot found" }
    }
    
    $snapshot = Get-Content $snapshotPath | ConvertFrom-Json
    
    # Check controller file
    $controllerHash = Get-FileHash $snapshot.controllerFile.Path
    if ($controllerHash -ne $snapshot.controllerFile.hash) {
        return @{ NeedsUpdate = $true; Reason = "Controller file changed" }
    }
    
    # Check DTO files
    foreach ($dto in $snapshot.dtoFiles) {
        if (Test-Path $dto.path) {
            $currentHash = Get-FileHash $dto.path
            if ($currentHash -ne $dto.hash) {
                return @{ NeedsUpdate = $true; Reason = "DTO file changed: $($dto.path)" }
            }
        }
    }
    
    # Check test file
    $testHash = Get-FileHash $snapshot.testFile.Path
    if ($testHash -ne $snapshot.testFile.hash) {
        return @{ NeedsUpdate = $true; Reason = "Test file was modified" }
    }
    
    return @{ NeedsUpdate = $false }
}
```

## Location Conventions

- Test Project: `InternshipProjectSara.Tests/` (at project root)
- Controller Tests: `InternshipProjectSara.Tests/Controllers/`
- Service Tests: `InternshipProjectSara.Tests/Services/`
- Test Helpers: `InternshipProjectSara.Tests/Helpers/`
- Snapshots: `.opencode/snapshots/<resourceName>-tests.json`

## Rules

1. **Always check for existing test project first** - Don't recreate if it exists
2. **Validate before regenerating** - Check if tests are outdated or incomplete
3. **Maintain test isolation** - Each test should be independent
4. **Use proper mocking** - Mock all external dependencies
5. **Follow naming conventions** - `Method_Scenario_ExpectedResult`
6. **Include Arrange/Act/Assert** - Clear test structure
7. **Test all code paths** - Success, error, authorization
8. **Save snapshot after generation** - Enable change detection
9. **Don't modify source code** - Only generate/modify test files
10. **Handle edge cases** - Empty collections, null values, exceptions

## Output

The skill generates:
1. Test project (if not exists)
2. Controller test file with comprehensive tests
3. Test helper classes
4. Snapshot file for change detection
5. Validation report for existing tests

## Dependencies

- `parse-csharp-controller` - For extracting controller metadata
- `detect-backend-changes` - For checking if tests need regeneration
- Test frameworks: xUnit, Moq, FluentAssertions

## Current Limitations & Future Improvements

### Known Issues (Resolved)

1. **DTO Type Detection** - ~~The auto-generation script uses `object` instead of actual DTO types~~ **FIXED**: Script now parses controller method signatures to extract actual DTO types and generates proper test data instances.

2. **Method Parameter Handling** - ~~Methods with complex parameters (DTOs, multiple params) aren't handled correctly~~ **FIXED**: Script now handles mixed parameter lists (primitives + DTOs) by:
   - Using `Build-FullParamCall()` to generate correct controller calls with ALL params in order
   - Using `Build-MockParamSetup()` to generate correct mock setups with ALL param types
   - For `Update(int id, UserRequestDto dto)`, now generates `_controller.Update(1, testUserRequestDto)` instead of `_controller.Update(testUserRequestDto)`

3. **Return Type Detection** - ~~The script doesn't extract return types from controller methods~~ **FIXED**: Script parses `ActionResult<T>` generic types for proper assertion types.

4. **Mock Field Naming** - ~~While fixed for constructor parameters, test methods still use generic names~~ **FIXED**: Mock setups now use correct field names from dependency metadata.

5. **Exception Test Pattern** - Controllers without try/catch blocks propagate exceptions. Exception tests now use `Record.Exception()` pattern instead of asserting on `result.Result` which would never be assigned.

6. **Role-Based Authorization** - `[Authorize(Roles="...")]` is enforced by ASP.NET pipeline, not by the controller. Role-based auth tests now generate proper placeholder tests noting this is tested via integration tests, instead of empty test bodies that could silently pass.

### Remaining Considerations

1. **Integration Test Coverage** - Role-based authorization (`[Authorize(Roles="...")]`) cannot be tested at the unit test level. Integration tests using `WebApplicationFactory` are needed for full authorization testing.

2. **Global Exception Handler** - If the project uses `UseExceptionHandler` middleware, exception tests verify that the exception propagates from the controller (unit test level). The middleware catches it at the pipeline level (integration test level).

### Recommended Improvements

1. **Enhanced Controller Parsing**
   ```powershell
   # Parse method signatures to extract:
   # - Return types (ActionResult<T>, IActionResult, etc.)
   # - Parameter types (DTOs, primitives)
   # - Generic type arguments
   ```

2. **DTO Import Generation**
   ```powershell
   # Auto-generate using statements for referenced DTOs
   # e.g., using InternshipProjectSara.Business.DTOs.Request;
   ```

3. **Test Data Generation**
   ```powershell
   # Generate realistic test data based on DTO properties
   # e.g., new UserRequestDto { Name = "Test User", Email = "test@example.com" }
   ```

4. **Integration with parse-csharp-controller**
   ```powershell
   # Use the parse-csharp-controller skill to extract structured metadata
   # Then use that metadata for test generation
   ```

### Manual Test Creation Workflow

For controllers with complex DTOs or parameters, use this workflow:

1. **Parse the controller manually** to understand:
   - Constructor dependencies
   - Method signatures (parameters and return types)
   - Authorization attributes

2. **Create test file manually** using the patterns in Phase 4

3. **Save snapshot** for change detection:
   ```powershell
   # Run validation to save snapshot
   .\Invoke-UnitTestGeneration.ps1 -ControllerName "UserController" -ValidateOnly
   ```

4. **Use change detection** to know when to update tests:
   ```powershell
   # Check for backend changes
   .\Invoke-UnitTestGeneration.ps1 -ControllerName "UserController" -ValidateOnly
   # If "Changes detected" - manual update needed
   ```

## Test File Naming Convention

- **Auto-generated**: `{ControllerName}ControllerTests.cs`
- **Manual tests**: `{ControllerName}ControllerTests.cs` (same convention)
- **Snapshot**: `.opencode/snapshots/{ControllerName}_Tests.json`

## Validation Report Location

- **Reports**: `InternshipProjectSara.Tests/ValidationReports/{ControllerName}_ValidationReport.json`
- **Snapshots**: `.opencode/snapshots/{ControllerName}_Tests.json`
