---
name: uica-table
description: UICA table/list generation skill for .NET controllers. Use when creating index/list views for an ASP.NET Core controller. Generates sortable, filterable, paginated data tables working with IQueryable/OData/paged endpoints.
---

# UICA Table Generation (.NET)

## Table structure

For a controller's list/index endpoint (`GET /api/products` or `GET /api/products?page=1&size=10`), generate:

1. **Toolbar** — Search input, filter dropdowns, date range picker, "Add New" button
2. **Table/List** — Column headers, data rows, row actions
3. **Pagination** — Page controls with page size selector
4. **Bulk actions** — Select-all checkbox, batch delete/export

## Column mapping from C# response types

Map ASP.NET Core controller response DTO properties to table columns:

| C# property type | Column display |
|---|---|
| `string` | Text column, sortable, searchable |
| `int`, `long`, `decimal`, `double` | Right-aligned number, sortable, filterable by range |
| `bool` | Checkmark/cross icon, badge, or toggle |
| `DateTime`, `DateTimeOffset`, `DateOnly` | Formatted date (locale-aware), sortable, filterable by range |
| `Guid` | Truncated ID (`"…abc123"`) with copy tooltip, or hidden |
| `Enum` | Colored badge/tag (map `Description` or `Display` attributes) |
| `string` with `[EmailAddress]` | Mailto link |
| `string` with `[Url]` | External link icon |
| `List<T>`, `IEnumerable<T>` | Comma-separated or first-N with "+N more" badge |
| `ICollection<T>` navigation prop | Count badge ("3 items") |
| `T?` (nullable) | Show "—" dash for null |
| `decimal` with `[DataType(DataType.Currency)]` | Formatted currency (`$42.00`) |
| `byte[]` | Avatar/thumbnail preview |
| Custom type with `ToString()` | Formatted display value |

## Action columns

Match controller action methods to row actions:

| Controller action | Table action |
|---|---|
| `GetById(id)` → `[HttpGet("{id}")]` | "View" link or row click → detail page |
| `Update(id, dto)` → `[HttpPut("{id}")]` | "Edit" button → edit form/modal |
| `Delete(id)` → `[HttpDelete("{id}")]` | "Delete" button → confirm dialog with "Are you sure?" |
| `Approve(id)` → `[HttpPost("{id}/approve")]` | "Approve" action button → confirm → toast |
| `Export()` → `[HttpGet("export")]` | "Export" button → download CSV/Excel/PDF |
| Custom action methods | Action buttons per custom endpoint |

## .NET pagination patterns

Handle common .NET pagination patterns:

### Page-based (most common):
```
GET /api/products?page=1&pageSize=10
→ { items: [...], totalCount: 42, page: 1, pageSize: 10, totalPages: 5 }
```
Generate: page selector, "1-10 of 42" summary, page size dropdown

### OData-style:
```
GET /api/products?$top=10&$skip=0&$orderby=name asc&$filter=contains(name,'foo')
```
Generate: column sorting → `$orderby`, search → `$filter`, pagination → `$top`/`$skip`

### Cursor-based:
```
GET /api/products?cursor=abc123&limit=10
→ { items: [...], nextCursor: "def456", hasMore: true }
```
Generate: "Load more" button or infinite scroll

### IQueryable enabled:
If the controller returns `IQueryable<T>` via OData or `[EnableQuery]`:
- Server handles sorting, filtering, and pagination
- Generate URL parameter builders for `$filter`, `$orderby`, `$top`, `$skip`

## States

Every table must handle:
- **Loading** — Skeleton rows or spinner
- **Empty** — Friendly empty state with CTA ("No products yet. Add your first product.")
- **Error** — Error banner with retry button (handle `ProblemDetails` response)
- **Data** — Normal data display
- **Refreshing** — Show stale data with subtle loading indicator
- **Filtered empty** — "No results match your filters" with clear-filters action

## Sorting & filtering

- Sortable columns: Click header toggles asc/desc/none
- Pass sort as `?sortBy=name&sortDir=asc` (or OData `$orderby`)
- Debounce search input (300ms)
- Generate filter controls matching C# property types:
  - `string` → text input
  - `Enum` → multi-select dropdown
  - `bool` → yes/no/any tri-state
  - `DateTime` → date range picker
  - `decimal` → min/max range inputs
- Show filter chips with remove button
- "Clear all filters" action
- Sync sort/filter/page state to URL query params for shareable URLs

## Example templates

### SPA table with typed API service:
```
<TableToolbar />        ← Search, filters, "Add New" button
<TableHeader />         ← Sortable column headers per DTO properties
<TableBody />           ← Data rows typed to C# DTO
<TablePagination />     ← Page controls matching API paging model
<BulkActions />         ← Batch delete, export selected
```

### Blazor table:
```
<QuickGrid Items="@products">
  <PropertyColumn Property="@(p => p.Name)" Sortable="true" />
  <PropertyColumn Property="@(p => p.Price)" Format="C2" Sortable="true" />
  <TemplateColumn Title="Actions">
    <button @onclick="() => Edit(p.Id)">Edit</button>
    <button @onclick="() => Delete(p.Id)">Delete</button>
  </TemplateColumn>
</QuickGrid>
```
