---
description: Generate a validated form UI for a .NET controller's create/update action. Pass the controller method or DTO model path.
agent: uica
---

Generate a validated form user interface for the following .NET controller action:

$ARGUMENTS

Focus specifically on the form component:
1. Read the C# DTO / ViewModel and map its properties to appropriate form inputs
2. Mirror Data Annotations (`[Required]`, `[StringLength]`, `[Range]`, etc.) or FluentValidation rules as client-side validation
3. Use `[Display(Name)]` for labels and `[Display(Prompt)]` for placeholders
4. Support both create and edit modes via optional initialData prop
5. Include loading, submitting, success, and error states
6. API integration for POST (create) and PUT/PATCH (edit) endpoints
7. Parse `ValidationProblemDetails` (400) for field-level server errors
8. Handle `ProblemDetails` (500) for general server errors

Examine existing form components in the project to match conventions.
