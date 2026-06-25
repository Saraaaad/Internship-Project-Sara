---
description: Generate a complete CRUD UI for an ASP.NET Core controller. Pass the controller file path or action definitions.
agent: uica
---

Generate a complete CRUD user interface for the following .NET controller:

$ARGUMENTS

Read the C# controller file. Analyze its `[ApiController]` attributes, action methods, Data Annotations / FluentValidation rules, and DTO models. Then create:
1. TypeScript interfaces matching the C# DTOs (or C# view models for Blazor)
2. Typed HTTP client service matching each controller action
3. API integration hooks / services (using the project's data-fetching pattern)
4. Index/List page with sortable, filterable, paginated table
5. Create form validating against Data Annotations / FluentValidation rules
6. Edit form (reuse create form populated with initial data)
7. Detail/show view
8. Delete confirmation dialog
9. Page-level component composing all sub-components
10. Route/page setup

Handle .NET-specific concerns:
- Map `ProblemDetails` / `ValidationProblemDetails` error responses
- Use camelCase JSON property naming matching ASP.NET Core serialization
- Support nullable reference types correctly
- Handle `Guid` ID routing in URLs
- Support pagination matching the API's paging model

Examine existing UI code first to match conventions exactly.
