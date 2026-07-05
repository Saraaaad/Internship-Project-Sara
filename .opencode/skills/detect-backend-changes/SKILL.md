---
name: detect-backend-changes
description: Detect whether a controller's source files have changed since the last UI generation. Compares current file content hashes against stored snapshots. Use ONLY before generating UI to avoid redundant regeneration. Do NOT parse or generate UI code.
---

# Detect Backend Changes

Compare the current source files of a controller against a stored snapshot. Report whether regeneration is needed.

## How it works

1. Identify all source files for the controller:
   - The controller `.cs` file at `Application/Controllers/<Name>Controller.cs`
   - All referenced DTO files (read imports and parameter types to find them)
   - All referenced enum files

2. Compute a SHA256 content hash for each file.

3. Load the previous snapshot from `.opencode/snapshots/<resourceName>.json`.
   The snapshot stores:
   ```json
   {
     "generatedAt": "ISO-8601 timestamp",
     "files": {
       "Application/Controllers/UserController.cs": "sha256hash",
       "Business/DTOs/Request/UserRequestDto.cs": "sha256hash",
       "Business/DTOs/Response/UserResponseDto.cs": "sha256hash",
       "Shared/Enums/Role.cs": "sha256hash"
     }
   }
   ```

4. Compare hashes. Report results:
   - `hasChanges: true/false`
   - `changedFiles[]`: list of files whose hash differs
   - `addedFiles[]`: files in current not in snapshot
   - `removedFiles[]`: files in snapshot not in current
   - `needsRegeneration`: true if any change is detected

5. If no snapshot exists → `hasChanges: true` with reason "first generation"

## Snapshot management

- After generating UI, save a new snapshot to `.opencode/snapshots/<resourceName>.json`.
- Overwrite the previous snapshot each time.

## Rules

- Do NOT parse the controller logic or generate any UI code.
- Do NOT modify any source files.
- Only read files and compare hashes.
- If a file doesn't exist (e.g., optional DTO), skip it gracefully.
