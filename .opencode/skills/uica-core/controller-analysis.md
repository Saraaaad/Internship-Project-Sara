---
name: uica-core
description: Core UICA skill for generating UI from .NET ASP.NET Core controllers. Use when creating any controller UI - provides C# controller analysis, [ApiController] attribute parsing, pattern selection, and project-aware generation strategies.
---

# UICA Core

## .NET Controller analysis

When given an ASP.NET Core controller (C# file), extract:

### Route info from attributes
- `[Route("api/[controller]")]` → `/api/products`
- `[HttpGet]`, `[HttpGet("{id}")]` → GET endpoint
- `[HttpPost]` → POST endpoint
- `[HttpPut("{id}")]` → PUT endpoint
- `[HttpPatch("{id}")]` → PATCH endpoint
- `[HttpDelete("{id}")]` → DELETE endpoint

### Parameter binding attributes
- `[FromBody]` → request body (POST/PUT)
- `[FromQuery]` → query string parameter
- `[FromRoute]` → URL path parameter
- `[FromForm]` → form file/data upload
- `[FromHeader]` → header value

### .NET type → UI type mapping
| C# type | UI representation |
|---|---|
| `string` | Text input |
| `int`, `long`, `short` | Number input |
| `decimal`, `double`, `float` | Decimal number input |
| `Guid` | Hidden or read-only text (auto-generated) |
| `bool` | Checkbox / toggle |
| `DateTime`, `DateTimeOffset` | Date picker, date-time picker |
| `DateOnly` | Date picker (no time) |
| `TimeOnly` | Time picker |
| `Enum` | Select dropdown |
| `List<T>`, `IEnumerable<T>`, `ICollection<T>` | Multi-select, dynamic list, or table |
| `IFormFile` | File upload with drag-drop |
| `byte[]` | File upload or image preview |
| `Dictionary<string, string>` | Key-value input pairs |
| `T?` (nullable) | Optional field, append "None" option |

### Validation rules from Data Annotations
| Attribute | Client-side equivalent |
|---|---|
| `[Required]` | `required`, `aria-required` |
| `[StringLength(max)]` | `maxLength` |
| `[StringLength(min, max)]` | `minLength` + `maxLength` |
| `[Range(min, max)]` | `min` + `max` (number) |
| `[EmailAddress]` | `type="email"` |
| `[Phone]` | `type="tel"` |
| `[Url]` | `type="url"` |
| `[RegularExpression(pattern)]` | `pattern` regex validation |
| `[Compare("other")]` | Confirm field match |
| `[MinLength(n)]` | `minLength` |
| `[MaxLength(n)]` | `maxLength` |
| `[CreditCard]` | Payment card format |
| Custom `IValidatableObject` | Custom validation message |

For FluentValidation projects, map equivalent rules (`RuleFor(x => x.Name).NotEmpty().MaximumLength(100)`).

### Response types
- `ActionResult<T>`, `IActionResult`, `Task<IActionResult>` → Determine success/error shape
- `Ok(result)` → 200 with body
- `CreatedAtAction(nameof(Get), new { id }, resource)` → 201 with location
- `BadRequest(error)` → 400 with validation errors
- `NotFound()` → 404
- `Problem(detail)` → Problem Details (RFC 7807)
- `PagedResult<T>`, `PagedResponse<T>` → Paginated list response

### Common .NET controller patterns
- **Entity Framework CRUD** — `DbContext` injected, standard CRUD with `DbSet<T>`
- **MediatR / CQRS** — `IMediator.Send(command)` pattern
- **Repository pattern** — `IRepository<T>` injected
- **Service layer** — `IService` injected, controller delegates to service

## UI pattern selection

| Controller type | UI pattern | Components |
|---|---|---|
| Full CRUD resource controller | Resource management | IndexTable, CreateForm, EditForm, ShowDetail, DeleteDialog |
| Read-only resource (no create/update/delete) | Data browser | IndexTable, ShowDetail, SearchBar, FilterPanel |
| Single-action (e.g., POST /approve) | Action trigger | ActionButton, ConfirmDialog, ResultDisplay |
| Auth / Identity controller | Auth flow | LoginForm, RegisterForm, ForgotPasswordForm, ResetPasswordForm |
| Dashboard / aggregation endpoint | Dashboard | StatCard, ChartWidget, DataTable, DateRangePicker |
| File upload / storage controller | File manager | FileUpload, FileList, FilePreview, DropZone |
| Webhook / event controller | Event log | EventList, EventDetail, EventFilter |
| Health / status endpoint | Status page | HealthIndicator, StatusBadge, MetricsGrid |
| OData / queryable endpoint | Advanced table | QueryableTable, AdvancedFilterPanel, ColumnSelector |

## Structure generation

For an ASP.NET Core `ProductsController` at `Controllers/ProductsController.cs`:

### If using Blazor:
```
Pages/Products/
├── Components/
│   ├── ProductTable.razor        # Index view
│   ├── ProductForm.razor         # Create + Edit form
│   ├── ProductDetail.razor       # Show view
│   └── ProductDeleteDialog.razor # Confirm delete
├── ProductIndex.razor            # Index page
├── ProductCreate.razor           # Create page
├── ProductEdit.razor             # Edit page
└── ProductService.cs             # HTTP client service
```

### If using SPA (React/Angular/Vue):
```
src/features/products/
├── components/
│   ├── ProductTable.tsx        # Index view
│   ├── ProductForm.tsx         # Create + Edit form
│   ├── ProductDetail.tsx       # Show view
│   └── ProductDeleteDialog.tsx # Confirm delete
├── hooks/
│   └── useProducts.ts          # Data fetching hooks
├── api/
│   └── products.ts             # API client (typed HttpClient)
├── types/
│   └── product.ts              # TypeScript interfaces matching C# DTOs
├── ProductPage.tsx             # Page-level orchestration
└__ tests__/
    ├── ProductTable.test.tsx
    ├── ProductForm.test.tsx
    └── ProductDetail.test.tsx
```

## Project-aware generation

Before generating, examine the project to determine:
1. **Frontend framework** — Blazor Server, Blazor WASM, React, Angular, Vue, or Razor Pages
2. **Blazor specifics** — SSR vs WASM, render mode (`InteractiveServer`, `InteractiveWebAssembly`, `InteractiveAuto`), `@rendermode` directives
3. **Styling** — Tailwind CSS, Bootstrap, MudBlazor, Fluent UI Blazor, MatBlazor, CSS Modules
4. **API client approach** — Generated NSwag/OpenAPI client, typed `HttpClient` services, `IOptions<EndpointSettings>`
5. **.NET project structure** — Solution files, project references, folder conventions (Controllers/, Services/, Models/, DTOs/)
6. **Form validation sync** — Check if using Data Annotations or FluentValidation; mirror rules client-side
7. **UI library** — MudBlazor, Radzen, Blazorise, Syncfusion, Telerik, MUI, Ant Design, PrimeNG
8. **State management** — Fluxor (Blazor), Redux, Zustand, NgRx, Pinia
9. **Authentication** — ASP.NET Core Identity, JWT bearer, OAuth, Azure AD
10. **Testing** — bUnit (Blazor), Jest, Vitest, React Testing Library

Always look at 2-3 existing UI components in the project to match exact conventions.
