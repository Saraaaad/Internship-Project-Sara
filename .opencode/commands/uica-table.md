---
description: Generate a data table/list view for a .NET controller's index endpoint. Pass the controller index action path.
agent: uica
---

Generate a data table / list view for the following .NET controller index action:

$ARGUMENTS

Focus specifically on the table component:
1. Map the C# response DTO properties to table columns with proper formatting
2. Search bar with debounced input
3. Sortable column headers matching the API's sort parameter convention
4. Filter controls matching searchable/filterable C# property types
5. Pagination matching the API's paging model (page-based, OData, or cursor)
6. Row actions (view, edit, delete) corresponding to controller CRUD actions
7. Loading skeleton, empty state, error state, filtered-empty state
8. Bulk selection with batch operations (delete, export)
9. Responsive behavior (card layout on mobile)

Handle .NET specifics:
- Map `ProblemDetails` errors to user-friendly messages
- Parse paginated responses (items + totalCount structure)
- Support OData-style `$filter`, `$orderby`, `$top`, `$skip` when applicable

Examine existing table components in the project to match conventions.
