---
name: generate-form-for-controller
description: Generate a single create/edit form component from parsed controller metadata. Maps .NET Data Annotations to client-side validation. Use ONLY when generating a form for one controller action. Do NOT generate tables, pages, or API clients.
---

# Generate Form for Controller

Generate one form component for a controller's create or update action.

## Input

You receive parsed controller metadata (from `parse-csharp-controller`):
- The controller's route prefix, resource name
- The create action (`[HttpPost]`) and/or update action (`[HttpPut]`, `[HttpPatch]`)
- The request DTO with all properties and validation rules
- Enums referenced by the DTO

## What to generate

### 1. Form component
- A single component handling both create and edit modes
- Accept optional `initialData` prop — populate fields for editing
- Map each DTO property to a form input using this table:

| C# type + annotations | Input type |
|---|---|
| `string`, no special annotation | `<input type="text">` |
| `string` + `[EmailAddress]` | `<input type="email">` |
| `string` + `[Url]` | `<input type="url">` |
| `string` + `[Phone]` | `<input type="tel">` |
| `string` + `[DataType(DataType.Password)]` | `<input type="password">` |
| `string` + `[DataType(DataType.MultilineText)]` | `<textarea>` |
| `int`, `long`, `short` | `<input type="number">` |
| `decimal`, `double`, `float` | `<input type="number" step="any">` |
| `bool` | Checkbox or toggle switch |
| `DateTime`, `DateTimeOffset`, `DateOnly` | `<input type="date">` or date picker |
| `TimeOnly` | `<input type="time">` |
| `Guid` | Hidden input (auto-generated) |
| `Enum` | `<select>` dropdown with all enum values |
| `List<T>`, `IEnumerable<T>` | Multi-select or checkbox group |
| `IFormFile` | `<input type="file">` with drag-drop |
| Nullable `T?` | Optional field with "None" option |

### 2. Client-side validation mirroring Data Annotations
| Data Annotation | HTML/client validation |
|---|---|
| `[Required]` | `required` attribute |
| `[StringLength(max)]` | `maxlength` attribute |
| `[StringLength(min, max)]` | `minlength` + `maxlength` |
| `[Range(min, max)]` | `min` + `max` attributes |
| `[EmailAddress]` | `type="email"` |
| `[RegularExpression("pat")]` | `pattern` attribute |
| `[MinLength(n)]` | `minlength` |
| `[MaxLength(n)]` | `maxlength` |
| `[Compare("other")]` | Matching confirm field |

### 3. Form states
- **Idle**: empty form, ready for input
- **Submitting**: spinner, fields disabled, button shows "Saving..."
- **Success**: toast/notification, optional redirect
- **Server error**: parse `ValidationProblemDetails` (400) for field errors
- **Server error**: parse `ProblemDetails` (500) for general errors

### 4. API integration
- Match `[Display(Name = "...")]` to label text
- Match `[Display(Prompt = "...")]` to placeholder
- Error messages from `[ErrorMessage = "..."]` as validation messages
- For create: `POST {routePrefix}` with body
- For update: `PUT {routePrefix}/{id}` with body + route param

## Location

Save to: `Frontend/{resourceName}/Components/{resourceName}Form.tsx`
(or `.razor` for Blazor)

## Do NOT

- Generate tables, lists, or index views
- Generate API client services (that's a different skill)
- Generate page-level routing or navigation
- Parse the controller file (use the parsed metadata already provided)
