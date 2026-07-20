---
name: generate-form-for-controller
description: Generate a single create/edit form component from parsed controller metadata. Maps .NET Data Annotations to client-side validation. Use ONLY when generating a form for one controller action. Do NOT generate tables, pages, or API clients.
---

# Generate Form for Controller

Generate one form component for a controller's create or update action.

## Frontend Stack

- **React 19** + **TypeScript** functional components
- CSS classes (not CSS modules, not Tailwind, not styled-components)
- No external UI libraries — pure HTML + CSS
- **sonner** for toast notifications (`import { toast } from 'sonner'`)
- Controlled inputs with `useState`

## Input

You receive parsed controller metadata (from `parse-csharp-controller`):
- The controller's route prefix, resource name
- The create action (`[HttpPost]`) and/or update action (`[HttpPut]`, `[HttpPatch]`)
- The request DTO with all properties and validation rules
- Enums referenced by the DTO

## What to generate

### 1. Component structure

**Location:** `Frontend/src/components/{ResourceName}Form.tsx`

```typescript
import React, { useState, useEffect } from 'react';
import { toast } from 'sonner';
import type { RequestType } from '../types/{resourceName}.types';
import { {resourceName}Api, ApiError } from '../api/{resourceName}Api';

interface {ResourceName}FormProps {
  initialData?: RequestType;
  onSubmit: (dto: RequestType) => Promise<void>;
  onCancel: () => void;
  mode: 'create' | 'edit';
}

export const {ResourceName}Form: React.FC<{ResourceName}FormProps> = ({
  initialData,
  onSubmit,
  onCancel,
  mode,
}) => {
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  // Form state with initial values
  const [formData, setFormData] = useState<RequestType>({
    // Initialize all fields with empty/default values
    ...initialData,
  });

  // ... form implementation
};
```

### 2. Input mapping from C# types

| C# type + annotations | React input |
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
| `DateTime`, `DateTimeOffset`, `DateOnly` | `<input type="date">` |
| `TimeOnly` | `<input type="time">` |
| `Guid` | Hidden input (auto-generated) |
| `Enum` | `<select>` dropdown with all enum values |
| `List<T>`, `IEnumerable<T>` | Multi-select or checkbox group |
| Nullable `T?` | Optional field with "None" option |

### 3. Form input pattern

```tsx
<div className="form-group">
  <label htmlFor="fieldName">Field Name *</label>
  <input
    type="text"
    id="fieldName"
    name="fieldName"
    value={formData.fieldName}
    onChange={handleChange}
    required
    maxLength={100}
    placeholder="Enter field name"
    disabled={loading}
  />
  {errors.fieldName && <span className="field-error">{errors.fieldName}</span>}
</div>
```

### 4. Client-side validation from Data Annotations

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

Error messages from `[ErrorMessage = "..."]` as validation messages.

### 5. Form handler pattern

```typescript
const handleChange = (
  e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>
) => {
  const { name, value, type } = e.target;
  
  const checked = (e.target as HTMLInputElement).checked;
  
  setFormData((prev) => ({
    ...prev,
    [name]: type === 'checkbox' ? checked : type === 'number' ? Number(value) : value,
  }));

  // Clear field error on change
  if (errors[name]) {
    setErrors((prev) => {
      const next = { ...prev };
      delete next[name];
      return next;
    });
  }
};
```

### 6. Submit handler pattern

```typescript
const handleSubmit = async (e: React.FormEvent) => {
  e.preventDefault();
  
  // Client-side validation
  const validationErrors: Record<string, string> = {};
  
  if (!formData.name?.trim()) {
    validationErrors.name = 'Name is required';
  }
  // ... more validation rules from Data Annotations
  
  if (Object.keys(validationErrors).length > 0) {
    setErrors(validationErrors);
    toast.error('Please fix the errors below');
    return;
  }

  setLoading(true);
  try {
    await onSubmit(formData);
    toast.success(mode === 'create' ? '{ResourceName} created successfully' : '{ResourceName} updated successfully');
  } catch (error: any) {
    const errorMessage = error instanceof ApiError ? error.message : 'Operation failed';
    toast.error(errorMessage);
  } finally {
    setLoading(false);
  }
};
```

### 7. Form states

- **Idle**: empty form, ready for input
- **Submitting**: spinner, fields disabled, button shows "Saving..."
- **Success**: toast notification, optional redirect
- **Server error**: parse error message from `ApiError`

### 8. Form JSX pattern

```tsx
return (
  <form onSubmit={handleSubmit} className="{resourceName}-form">
    <div className="form-group">
      <label htmlFor="username">Username *</label>
      <input
        type="text"
        id="username"
        name="username"
        value={formData.username}
        onChange={handleChange}
        required
        placeholder="Enter username"
        disabled={loading}
      />
      {errors.username && <span className="field-error">{errors.username}</span>}
    </div>

    {/* More fields... */}

    <div className="form-actions">
      <button type="button" onClick={onCancel} className="btn btn-secondary" disabled={loading}>
        Cancel
      </button>
      <button type="submit" className="btn btn-primary" disabled={loading}>
        {loading ? 'Saving...' : mode === 'create' ? 'Create' : 'Update'}
      </button>
    </div>
  </form>
);
```

### 9. Enum select pattern

```tsx
<div className="form-group">
  <label htmlFor="role">Role *</label>
  <select
    id="role"
    name="role"
    value={formData.role}
    onChange={handleChange}
    required
    disabled={loading}
  >
    <option value="">Select Role</option>
    {(['Admin', 'HR', 'Lead', 'Employee'] as const).map((role) => (
      <option key={role} value={role}>{role}</option>
    ))}
  </select>
  {errors.role && <span className="field-error">{errors.role}</span>}
</div>
```

## CSS classes to use

- `.{resourceName}-form` — form container
- `.form-group` — field wrapper
- `.form-group label` — field label
- `.form-group input`, `.form-group select`, `.form-group textarea` — field inputs
- `.form-group .field-error` — validation error message
- `.form-actions` — button row
- `.btn` — base button
- `.btn-primary` — submit button
- `.btn-secondary` — cancel button
- `.loading-state` — loading indicator

## Location

Save to: `Frontend/src/components/{ResourceName}Form.tsx`

## Do NOT

- Generate table components (that's `generate-table-for-controller`)
- Generate API client services (that's `generate-api-client-for-controller`)
- Generate page-level layout or routing
- Parse the controller file (use parsed metadata already provided)
- Use external form libraries (react-hook-form, formik, etc.)
