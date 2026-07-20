---
name: parse-csharp-controller
description: Parse a C# ASP.NET Core controller file and extract structured metadata: route, HTTP methods, action signatures, parameter bindings, DTO references, and Data Annotation validation rules. Use ONLY when you need to read and interpret a .cs controller file for UI generation. Do NOT use for any other purpose.
---

# Parse C# Controller

Read a `.cs` file and extract these fields into a structured metadata object. **Do not generate any UI code.** Output only the metadata.

## Frontend Context

The metadata will be used to generate:
- TypeScript types in `Frontend/src/types/{resourceName}.types.ts`
- API client in `Frontend/src/api/{resourceName}Api.ts`
- React components in `Frontend/src/components/`
- Pages in `Frontend/src/pages/`
- CSS in `Frontend/src/assets/styles/`

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

### 5. TypeScript type mappings

For each DTO property, also output the TypeScript equivalent:

| C# type | TypeScript type |
|---|---|
| `string` | `string` |
| `int`, `long`, `short` | `number` |
| `decimal`, `double`, `float` | `number` |
| `bool` | `boolean` |
| `DateTime`, `DateTimeOffset` | `string` (ISO 8601) |
| `Guid` | `string` |
| `Enum` | Union type of string literals |
| `List<T>`, `IEnumerable<T>` | `T[]` |
| `Dictionary<K,V>` | `Record<K, V>` |
| `T?` | `T | null` |

### 6. Validation rule mappings

For each Data Annotation, output the HTML/React equivalent:

| Data Annotation | HTML attribute | React prop |
|---|---|---|
| `[Required]` | `required` | `required` |
| `[StringLength(max)]` | `maxlength` | `maxLength` |
| `[StringLength(min, max)]` | `minlength` + `maxlength` | `minLength` + `maxLength` |
| `[Range(min, max)]` | `min` + `max` | `min` + `max` |
| `[EmailAddress]` | `type="email"` | `type="email"` |
| `[Phone]` | `type="tel"` | `type="tel"` |
| `[Url]` | `type="url"` | `type="url"` |
| `[RegularExpression("pat")]` | `pattern` | `pattern` |
| `[MinLength(n)]` | `minlength` | `minLength` |
| `[MaxLength(n)]` | `maxlength` | `maxLength` |

## Output format

Return the metadata as a structured JSON-like object:

```json
{
  "controller": {
    "className": "UserController",
    "resourceName": "User",
    "routePrefix": "api/users",
    "resource": "users"
  },
  "actions": [
    {
      "actionName": "GetAll",
      "httpMethod": "GET",
      "route": "",
      "fullRoute": "api/users",
      "parameters": [],
      "responseType": "List<UserResponseDto>",
      "crudIntent": "list"
    }
  ],
  "dtos": [
    {
      "className": "UserRequestDto",
      "filePath": "Business/DTOs/Request/UserRequestDto.cs",
      "properties": [
        {
          "name": "Username",
          "type": "string",
          "isNullable": false,
          "validation": [
            { "attribute": "Required", "params": { "ErrorMessage": "Username is required" } }
          ],
          "typescriptType": "string",
          "htmlInputType": "text"
        }
      ]
    }
  ],
  "enums": [
    {
      "enumName": "Role",
      "values": ["Admin", "HR", "Lead", "Employee"],
      "typescriptUnion": "export type Role = 'Admin' | 'HR' | 'Lead' | 'Employee';"
    }
  ]
}
```

## Location conventions

- Controllers: `Application/Controllers/<Name>Controller.cs`
- Request DTOs: `Business/DTOs/Request/<Name>RequestDto.cs`
- Response DTOs: `Business/DTOs/Response/<Name>ResponseDto.cs`
- Enums: `Shared/Enums/<Name>.cs`

## Rules

- Only read and parse files — do not generate UI code
- Include all public actions with HTTP attributes
- Skip private/internal methods
- If a DTO file doesn't exist, report it but continue
- Include the TypeScript type mappings in the output for downstream skills
