---
description: Generate a data table/list view for a .NET controller's index endpoint. Pass the controller name or file path.
agent: uica
---

Generate a data table for the following controller:

$ARGUMENTS

Follow this workflow:
1. Load the `detect-backend-changes` skill to check if the controller or its response DTO changed.
2. Load the `parse-csharp-controller` skill to extract metadata from the controller file.
3. Load the `generate-table-for-controller` skill to generate the table component.
4. After generation, save the snapshot to `.opencode/snapshots/<resourceName>.json`.

Focus only on the table/list component for the index endpoint.
