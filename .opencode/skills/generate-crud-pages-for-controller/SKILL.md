---
name: generate-crud-pages-for-controller
description: Generate a complete set of CRUD pages from parsed controller metadata: index (table), create/edit (form), detail view, and delete dialog. Use ONLY when generating the full CRUD page set. Do NOT generate API clients — use generate-api-client-for-controller for that.
---

# Generate CRUD Pages for Controller

Generate the full set of CRUD pages for a controller that has list, create, read, update, and delete endpoints.

## Input

You receive parsed controller metadata (from `parse-csharp-controller`):
- All controller actions classified by CRUD intent
- All DTOs (request and response) with validation rules
- All referenced enums

## What to generate

### 1. Index page — `{resourceName}Page.tsx`
- Orchestrates everything: fetches data, manages state, handles navigation
- Renders the table component (delegate to `{resourceName}Table`)
- Floating "Add New" button or header action
- Wire delete confirmation dialog

### 2. Table component — `{resourceName}Table.tsx`
- See `generate-table-for-controller` skill for full specs.
- Sortable columns, search, filters, pagination
- Row actions: View, Edit, Delete

### 3. Form component — `{resourceName}Form.tsx`
- See `generate-form-for-controller` skill for full specs.
- Single component handling both create and edit modes
- Client-side validation mirroring Data Annotations
- Loading, submitting, success, error states

### 4. Detail component — `{resourceName}Detail.tsx`
- Read-only view of a single resource
- Display all response DTO properties with labels
- Loading state while fetching
- "Not found" state when 404
- "Edit" and "Delete" action buttons
- "Back to list" link

### 5. Delete dialog — `{resourceName}DeleteDialog.tsx`
- Modal/overlay confirming deletion
- Shows resource identifier (name or title)
- "Are you sure?" with resource type name
- Confirm button triggers DELETE endpoint
- Cancel button dismisses
- Loading state on delete
- Success state: close dialog, refresh list, show toast
- Error state: show error message

### 6. Component states (every component)
Every component must handle:
- **Loading**: skeleton or spinner
- **Empty** (list): friendly message with CTA
- **Error**: error message with retry button (parse `ProblemDetails`)
- **Success**: normal display

## Location

Save to: `Frontend/{resourceName}/`
```
Pages/{resourceName}Page.tsx          # Orchestration page
Components/{resourceName}Table.tsx    # Table (can delegate to generate-table skill output)
Components/{resourceName}Form.tsx     # Form (can delegate to generate-form skill output)
Components/{resourceName}Detail.tsx   # Detail view
Components/{resourceName}DeleteDialog.tsx  # Delete confirmation
```

## Do NOT

- Generate API client or HTTP service code (use `generate-api-client-for-controller` skill)
- Parse the controller file (use parsed metadata already provided)
- Generate test files (optional, if project has testing patterns)
