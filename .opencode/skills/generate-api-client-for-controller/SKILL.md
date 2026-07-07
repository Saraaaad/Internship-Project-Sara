---
name: generate-api-client-for-controller
description: Generate a typed HTTP client service from parsed controller metadata. Creates TypeScript/C# service class with typed methods for each controller action. Use ONLY when generating API client code. Do NOT generate UI components.
---

# Generate API Client for Controller

Generate a typed HTTP client service matching every action in the controller.

## Input

You receive parsed controller metadata (from `parse-csharp-controller`):
- All actions with HTTP method, full route, parameter bindings, and response types
- All request DTOs as TypeScript interfaces (or C# if Blazor)
- All response DTOs as TypeScript interfaces (or C# if Blazor)
- All enums as TypeScript union types (or C# enums)

## What to generate

### 1. TypeScript interfaces matching C# DTOs
For each DTO referenced by the controller:
- Generate an interface with camelCase properties (ASP.NET Core serialization convention)
- Map C# types:
  - `string` → `string`
  - `int`, `long` → `number`
  - `decimal`, `double`, `float` → `number`
  - `bool` → `boolean`
  - `DateTime`, `DateTimeOffset` → `string` (ISO 8601)
  - `Guid` → `string`
  - `Enum` → union type of string literals
  - `List<T>` → `T[]`
  - `Dictionary<K,V>` → `Record<K, V>`
  - `T?` → `T | null`
- Add JSDoc comments from `[Display(Name = "...")]` and `[Required]` annotations

### 2. TypeScript enums
For each enum used by the controller:
```typescript
export type Role = 'Admin' | 'HR' | 'Lead' | 'Employee';
```

### 3. HTTP client service class
For each controller action, generate a typed method:
- `GET {fullRoute}` → method returning `Promise<TResponse>`
- `GET {fullRoute}` with query params → method with typed parameters
- `POST {fullRoute}` with `[FromBody]` → method accepting request DTO
- `PUT {fullRoute}` with body + route param → method accepting id + DTO
- `PATCH {fullRoute}` → method with partial body
- `DELETE {fullRoute}` → method returning `Promise<void>`

### 4. Error handling
- Parse HTTP 400 → throw typed `ValidationError` with field-level `errors` dictionary
- Parse HTTP 404 → throw `NotFoundError`
- Parse HTTP 500 → throw `ServerError` with message from `ProblemDetails`
- Generic network errors → throw `NetworkError`

### 5. Service structure
Generate one service class per controller:
- Constructor accepts base URL and HTTP client
- All methods use camelCase matching action names
- Proper request/response headers (Content-Type: application/json)
- Cancellation token / AbortSignal support for request cancellation

## Location

Save to: `Frontend/api/{resourceName}Service.ts`
For Blazor: `Frontend/Services/{resourceName}Service.cs`

## Do NOT

- Generate UI components (forms, tables, pages)
- Generate HTML or component markup
- Parse the controller file (use parsed metadata already provided)
