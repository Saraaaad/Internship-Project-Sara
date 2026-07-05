---
description: Generate a complete CRUD UI for an ASP.NET Core controller. Creates index table, create/edit form, detail view, delete dialog, and API client.
agent: uica
---

Generate a complete CRUD UI for the following controller:

$ARGUMENTS

Follow this workflow:
1. Load the `detect-backend-changes` skill to check if the controller, its DTOs, or enums have changed.
2. Load the `parse-csharp-controller` skill to extract structured metadata from the controller file.
3. Load the `generate-api-client-for-controller` skill to create the typed HTTP client.
4. Load the `generate-crud-pages-for-controller` skill to create the full page set (table, form, detail, delete dialog).
5. After generation, save the snapshot to `.opencode/snapshots/<resourceName>.json`.

If the controller has NOT changed (detect-backend-changes says unchanged), skip generation and report "No changes detected — UI is up to date."
