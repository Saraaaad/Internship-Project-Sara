---
name: uica-form
description: UICA form generation skill for .NET controllers. Use when creating create/edit forms for an ASP.NET Core controller action. Maps C# Data Annotations to client-side validation and generates API integration.
---

# UICA Form Generation (.NET)

## Field mapping from C# types

Map ASP.NET Core controller DTO/ViewModel properties to form inputs:

| C# type + Data Annotations | Form input |
|---|---|
| `string` | `<input>` text |
| `string` + `[EmailAddress]` | `<input type="email">` |
| `string` + `[Url]` | `<input type="url">` |
| `string` + `[Phone]` | `<input type="tel">` |
| `string` + `[DataType(DataType.Password)]` | `<input type="password">` |
| `string` + `[DataType(DataType.MultilineText)]` | `<textarea>` |
| `string` (for rich text) | Rich text editor (TinyMCE, Quill, TipTap) |
| `int`, `long`, `short` | `<input type="number">` |
| `decimal`, `double`, `float` | `<input type="number" step="any">` or formatted currency |
| `bool` | `<input type="checkbox">` or toggle switch |
| `DateTime`, `DateTimeOffset`, `DateOnly` | `<input type="date">` or date picker |
| `TimeOnly` | `<input type="time">` |
| `Guid` | Hidden input (auto-generated server-side) |
| `Enum` | `<select>` dropdown with enum values |
| `List<T>`, `IEnumerable<T>` | Multi-select, tag input, or checkbox group |
| `ICollection<T>` with `[Required]` | Dynamic field array (add/remove items) |
| `IFormFile` | `<input type="file">` with drag-drop zone |
| `Dictionary<string, string>` | Key-value pair editor |
| Nullable `T?` | Optional field with "Not specified" option |

## Form structure guidelines

- Generate a single form component handling both create and edit modes
- Accept optional `initialData` prop — populate fields for editing
- Show mode indicator (Create vs Edit) matching .NET conventions
- Conditionally show/hide fields based on other selections
- Group related fields by C# model class hierarchy
- Map `[Display(Name = "...")]` to label text
- Use `[Display(Prompt = "...")]` as placeholder text
- Show `CharacterCount` for `[StringLength(max)]` fields

## Validation mirroring

| C# Data Annotation | Client validation |
|---|---|
| `[Required(ErrorMessage = "...")]` | Required validator with matching message |
| `[StringLength(100, MinimumLength = 3)]` | minLength=3 maxLength=100 with counter |
| `[Range(1, 100)]` | min=1 max=100 (number) |
| `[EmailAddress]` | Email regex pattern |
| `[RegularExpression(@"^[A-Z]+$")]` | Matching regex pattern |
| `[Compare("Password")]` | "Confirm" field must match |
| `[CreditCard]` | Luhn check |
| `[Phone]` | Phone format pattern |
| `[Url]` | URL format validation |
| `[MinLength(2)]` | minLength=2 |
| `[MaxLength(500)]` | maxLength=500 with counter |

For **FluentValidation** projects, map rules directly:
- `RuleFor(x => x.Name).NotEmpty()` → required
- `RuleFor(x => x.Age).InclusiveBetween(18, 120)` → range 18-120
- `RuleFor(x => x.Email).EmailAddress()` → email pattern

## States

Every form must handle:
- **Idle** — Empty form ready for input
- **Filling** — User typing, validation running
- **Submitting** — Loading spinner, fields disabled, button shows "Saving..."
- **Success** — Show success toast/message, optionally redirect to index
- **Server error** — Parse `ProblemDetails` RFC 7807 response, show field-level `errors` dictionary
- **Validation error** — Highlight invalid fields with server-returned messages

## API integration

- Generate typed HTTP client service for the controller
- For create: `POST /api/products` → `HttpResponseMessage` or typed response
- For update: `PUT /api/products/{id}` with route parameter
- Handle `400 Bad Request` — Parse `ValidationProblemDetails` for field errors
- Handle `404 Not Found` — Show "Resource not found" toast
- Handle `500` — Show generic error with correlation ID from `ProblemDetails`
- Map C# `camelCase` JSON serialization in TypeScript types
- For Blazor: inject `IHttpClientFactory` or generated service client
- For SPAs: generate TypeScript service class with typed methods

## Example templates

### SPA (React/Vue/Angular):
```
<FormFields />          ← Fields mapped from C# DTO properties
<FormActions />         ← Submit + Cancel buttons
<FormErrors />          ← ValidationProblemDetails display
<FormLoading />         ← Loading overlay
```

### Blazor:
```
<EditForm Model="@model">
  <DataAnnotationsValidator />
  <ValidationSummary />
  <InputText @bind-Value="model.Name" />
  ...
  <button type="submit">Save</button>
</EditForm>
```
