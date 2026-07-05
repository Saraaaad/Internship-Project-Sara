---
description: Generate a validated form UI for a .NET controller's create/update action. Pass the controller name or file path.
agent: uica
---

Generate a validated form for the following controller:

$ARGUMENTS

Follow this workflow:
1. Load the `detect-backend-changes` skill to check if the controller or its request DTO changed.
2. Load the `parse-csharp-controller` skill to extract metadata from the controller file.
3. Load the `generate-form-for-controller` skill to generate the form component.
4. After generation, save the snapshot to `.opencode/snapshots/<resourceName>.json`.

Focus only on the form component for the create/update action.
