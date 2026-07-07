---
name: parse-csharp-controller
description: Parse a C# ASP.NET Core controller file and extract structured metadata: route, HTTP methods, action signatures, parameter bindings, DTO references, and Data Annotation validation rules. Use ONLY when you need to read and interpret a .cs controller file for UI generation. Do NOT use for any other purpose.
---

# Parse C# Controller

Read a `.cs` file and extract these fields into a structured metadata object. **Do not generate any UI code.** Output only the metadata.

## What to extract

### 1. Controller info
- `className`: the class name (e.g., `UserController`)
- `resourceName`: the resource name without `Controller` suffix (e.g., `User`)
- `routePrefix`: the `[Route]` attribute value (e.g., `api/users`)
- `resource`: the resource part of the route (e.g., `users`)

### 2. Each action method
For each public method with an HTTP attribute, extract:
- `actionName`: method name (e.g., `GetAll`, `Create`)
- `httpMethod`: GET/POST/PUT/PATCH/DELETE from attribute
- `route`: relative route from the action's attribute (e.g., `{id}`, `employee/{employeeId}`, empty for default)
- `fullRoute`: combine `routePrefix` + `route` (e.g., `api/users/{id}`)
- `parameters[]`: for each parameter:
  - `name`, `type` (C# type name)
  - `binding`: `FromBody`, `FromQuery`, `FromRoute`, `FromForm`, or implicit
  - `isRequired`: true/false based on `[Required]` or non-nullable
  - `validation[]`: list of Data Annotation attributes as `{attribute: string, params: {key: value}}`
- `responseType`: `ActionResult<T>` type argument, or `void`/`IActionResult`
- `crudIntent`: infer from method name — GetAll/List → `list`, GetById/Get → `detail`, Create → `create`, Update/Edit → `update`, Delete → `delete`

### 3. Referenced DTOs
For each DTO referenced in an action (as `[FromBody]` parameter or response type), read the DTO `.cs` file and extract:
- `className`: the DTO class name
- `filePath`: relative path to the DTO file
- `properties[]`: for each property:
  - `name`, `type` (C# type)
  - `isNullable`: true/false
  - `validation[]`: Data Annotations as `{attribute: string, params: {key: value}}`
  - `displayName`: from `[Display(Name = "...")]`
  - `placeholder`: from `[Display(Prompt = "...")]`

### 4. Enum references
For any enum type used in DTOs, read the enum `.cs` file and extract:
- `enumName`: the enum name
- `values[]`: list of enum member names as strings

## Output format

Return the metadata as a structured JSON-like object. The agent will pass this to other skills for UI generation.

## Location conventions

- Controllers: `Application/Controllers/<Name>Controller.cs`
- Request DTOs: `Business/DTOs/Request/<Name>RequestDto.cs`
- Response DTOs: `Business/DTOs/Response/<Name>ResponseDto.cs`
- Enums: `Shared/Enums/<Name>.cs`
