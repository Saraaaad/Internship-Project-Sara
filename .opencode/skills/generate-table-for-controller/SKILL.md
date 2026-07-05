---
name: generate-table-for-controller
description: Generate a single table/list component from parsed controller metadata. Renders a sortable, filterable, paginated data table for the list/index endpoint. Use ONLY when generating a table view. Do NOT generate forms, pages, or API clients.
---

# Generate Table for Controller

Generate one table component for a controller's list/index action.

## Input

You receive parsed controller metadata (from `parse-csharp-controller`):
- The controller's route prefix, resource name
- The list action (`[HttpGet]` returning a collection)
- The detail action (`[HttpGet("{id}")]`) — for row click navigation
- The update action — for "Edit" row button
- The delete action — for "Delete" row button
- The response DTO with all properties
- Enums referenced by the DTO

## What to generate

### 1. Table component
- Toolbar: search input (debounced 300ms), filter controls, "Add New" button linking to create page
- Column headers mapped from response DTO properties:

| C# type | Column display |
|---|---|
| `string` | Text, sortable, searchable |
| `int`, `long`, `decimal`, `double` | Right-aligned, sortable |
| `bool` | Checkmark/badge |
| `DateTime`, `DateTimeOffset`, `DateOnly` | Formatted date, sortable |
| `Guid` | Truncated with copy tooltip |
| `Enum` | Colored badge (map enum names to display) |
| Nullable `T?` | Show "—" for null |
| `List<T>`, `IEnumerable<T>` | First-N with "+N more" badge |
| `decimal` (currency) | Formatted currency |
| `byte[]` | Thumbnail/avatar |

- Action column with buttons matching controller actions:
  - GET by id → "View" button/link
  - PUT/PATCH → "Edit" button
  - DELETE → "Delete" button with confirm dialog
- Pagination matching the API's paging model:
  - Page-based: page numbers, "1-10 of 42", page size dropdown
  - OData-style: `$top`/`$skip` controls
  - Cursor-based: "Load more" button
- Loading skeleton while data loads
- Empty state with CTA when no data
- Error state with retry button
- Filtered-empty state with "Clear filters" action

### 2. Sorting and filtering
- Sortable columns: clicking header toggles asc/desc/none
- Send sort as `?sortBy=name&sortDir=asc`
- Debounce search input (300ms)
- Filter controls per property type:
  - `string` → text input
  - `Enum` → multi-select dropdown
  - `bool` → yes/no/any tri-state
  - `DateTime` → date range picker
  - `decimal` → min/max range inputs
- Show active filter chips with remove buttons
- Sync sort/filter/page to URL query params

### 3. API integration
- GET `{routePrefix}?page=1&pageSize=10&search=...&sortBy=...&sortDir=...` → typed response
- Parse paginated response: `{ items: T[], totalCount: number, page: number, pageSize: number }`
- Handle `ProblemDetails` error responses

## Location

Save to: `Frontend/{resourceName}/Components/{resourceName}Table.tsx`
(or `.razor` for Blazor)

## Do NOT

- Generate form components (that's `generate-form-for-controller`)
- Generate API client services (that's `generate-api-client-for-controller`)
- Generate page-level layout or routing
- Parse the controller file (use parsed metadata already provided)
