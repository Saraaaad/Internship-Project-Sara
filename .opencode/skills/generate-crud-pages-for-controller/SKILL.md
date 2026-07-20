---
name: generate-crud-pages-for-controller
description: Generate a complete set of CRUD pages from parsed controller metadata: index (table), create/edit (form), detail view, and delete dialog. Use ONLY when generating the full CRUD page set. Do NOT generate API clients — use generate-api-client-for-controller for that.
---

# Generate CRUD Pages for Controller

Generate the full set of CRUD pages for a controller that has list, create, read, update, and delete endpoints.

## Frontend Stack

- **React 19** + **TypeScript** + **Vite**
- **No axios** — use native `fetch` API
- **No router** — conditional rendering based on auth state
- **sonner** for toast notifications (`import { toast } from 'sonner'`)
- CSS classes (not CSS modules, not Tailwind)
- Auth token in `localStorage` under key `token`

## Project Structure

```
Frontend/
├── src/
│   ├── api/                    # API client modules
│   │   ├── logsApi.ts          # Existing
│   │   └── {resource}Api.ts    # Generated
│   ├── types/                  # TypeScript type definitions
│   │   ├── log.types.ts        # Existing
│   │   └── {resource}.types.ts # Generated
│   ├── components/             # Reusable components
│   │   ├── LogTable.tsx        # Existing
│   │   ├── LogFilters.tsx      # Existing
│   │   ├── LogsStats.tsx       # Existing
│   │   └── {Resource}*.tsx     # Generated
│   ├── pages/                  # Page-level components
│   │   ├── LogsPage.tsx        # Existing
│   │   ├── LoginPage.tsx       # Existing
│   │   └── {Resource}Page.tsx  # Generated
│   ├── assets/
│   │   └── styles/             # Per-page CSS files
│   │       ├── LogsPage.css    # Existing
│   │       ├── LoginPage.css   # Existing
│   │       └── {Resource}Page.css # Generated
│   ├── App.tsx                 # Root component (auth routing)
│   ├── main.tsx                # Entry point
│   └── index.css               # Global styles
├── package.json
├── vite.config.ts
└── tsconfig.json
```

## Input

You receive parsed controller metadata (from `parse-csharp-controller`):
- All controller actions classified by CRUD intent
- All DTOs (request and response) with validation rules
- All referenced enums

## What to generate

### 1. Types file — `{resourceName}.types.ts`

**Location:** `Frontend/src/types/{resourceName}.types.ts`

```typescript
// Response DTO interface
export interface {ResourceName}Response {
  id: number;
  name: string;
  // ... all properties from response DTO
  createdAt: string;
}

// Request DTO interface
export interface {ResourceName}Request {
  name: string;
  // ... all properties from request DTO
}

// Enum types
export type Role = 'Admin' | 'HR' | 'Lead' | 'Employee';

// Helper functions (if needed)
export const getRoleBadge = (role: Role): string => {
  const badges: Record<Role, string> = {
    'Admin': 'badge-admin',
    'HR': 'badge-hr',
    'Lead': 'badge-lead',
    'Employee': 'badge-employee',
  };
  return badges[role] || 'badge-default';
};
```

### 2. API client — `{resourceName}Api.ts`

**Location:** `Frontend/src/api/{resourceName}Api.ts`

Generate using the `generate-api-client-for-controller` skill pattern. Include:
- `API_BASE_URL` constant
- `getAuthToken()` helper
- `getRequestOptions(method, body?)` helper
- `ApiError` class
- `handleResponse<T>(response)` helper
- Exported API object with typed methods

### 3. Table component — `{ResourceName}Table.tsx`

**Location:** `Frontend/src/components/{ResourceName}Table.tsx`

Generate using the `generate-table-for-controller` skill pattern. Include:
- Props interface with data, loading, callbacks
- Loading state
- Empty state with CTA
- Table with columns from response DTO
- Row actions (View, Edit, Delete)
- Optional: selection, sorting, filtering

### 4. Form component — `{ResourceName}Form.tsx`

**Location:** `Frontend/src/components/{ResourceName}Form.tsx`

Generate using the `generate-form-for-controller` skill pattern. Include:
- Props interface with initialData, onSubmit, onCancel, mode
- Controlled inputs with useState
- Client-side validation from Data Annotations
- Loading/disabled states
- Error handling with toast notifications

### 5. Detail component — `{ResourceName}Detail.tsx`

**Location:** `Frontend/src/components/{ResourceName}Detail.tsx`

Read-only view of a single resource:

```tsx
import React from 'react';
import type { {ResourceName}Response } from '../types/{resourceName}.types';

interface {ResourceName}DetailProps {
  item: {ResourceName}Response;
  onEdit: () => void;
  onDelete: () => void;
  onBack: () => void;
}

export const {ResourceName}Detail: React.FC<{ResourceName}DetailProps> = ({
  item,
  onEdit,
  onDelete,
  onBack,
}) => {
  return (
    <div className="{resourceName}-detail">
      <div className="detail-header">
        <h2>{ResourceName} Details</h2>
        <div className="detail-actions">
          <button onClick={onEdit} className="btn btn-primary">Edit</button>
          <button onClick={onDelete} className="btn btn-danger">Delete</button>
          <button onClick={onBack} className="btn btn-secondary">Back</button>
        </div>
      </div>
      
      <div className="detail-content">
        <div className="detail-row">
          <label>ID:</label>
          <span>{item.id}</span>
        </div>
        <div className="detail-row">
          <label>Name:</label>
          <span>{item.name}</span>
        </div>
        {/* More rows for each property */}
      </div>
    </div>
  );
};
```

### 6. Delete dialog — `{ResourceName}DeleteDialog.tsx`

**Location:** `Frontend/src/components/{ResourceName}DeleteDialog.tsx`

Modal confirming deletion:

```tsx
import React, { useState } from 'react';
import { toast } from 'sonner';
import { {resourceName}Api, ApiError } from '../api/{resourceName}Api';

interface {ResourceName}DeleteDialogProps {
  item: { id: number; name: string };
  isOpen: boolean;
  onClose: () => void;
  onDeleted: () => void;
}

export const {ResourceName}DeleteDialog: React.FC<{ResourceName}DeleteDialogProps> = ({
  item,
  isOpen,
  onClose,
  onDeleted,
}) => {
  const [loading, setLoading] = useState(false);

  if (!isOpen) return null;

  const handleDelete = async () => {
    setLoading(true);
    try {
      await {resourceName}Api.delete(item.id);
      toast.success('{ResourceName} deleted successfully');
      onDeleted();
      onClose();
    } catch (error: any) {
      const errorMessage = error instanceof ApiError ? error.message : 'Failed to delete';
      toast.error(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>Delete {ResourceName}</h2>
          <button className="modal-close" onClick={onClose}>✕</button>
        </div>
        <div className="modal-body">
          <p>Are you sure you want to delete <strong>{item.name}</strong>?</p>
          <p className="warning-text">This action cannot be undone.</p>
        </div>
        <div className="modal-footer">
          <button onClick={onClose} className="btn btn-secondary" disabled={loading}>
            Cancel
          </button>
          <button onClick={handleDelete} className="btn btn-danger" disabled={loading}>
            {loading ? 'Deleting...' : 'Delete'}
          </button>
        </div>
      </div>
    </div>
  );
};
```

### 7. Page component — `{ResourceName}Page.tsx`

**Location:** `Frontend/src/pages/{ResourceName}Page.tsx`

Orchestrates everything:

```tsx
import React, { useState, useEffect } from 'react';
import { toast } from 'sonner';
import { {ResourceName}Table } from '../components/{ResourceName}Table';
import { {ResourceName}Form } from '../components/{ResourceName}Form';
import { {ResourceName}Detail } from '../components/{ResourceName}Detail';
import { {ResourceName}DeleteDialog } from '../components/{ResourceName}DeleteDialog';
import { {resourceName}Api, ApiError } from '../api/{resourceName}Api';
import type { {ResourceName}Response } from '../types/{resourceName}.types';
import '../assets/styles/{ResourceName}Page.css';

type ViewMode = 'list' | 'create' | 'edit' | 'detail';

export const {ResourceName}Page: React.FC = () => {
  const [items, setItems] = useState<{ResourceName}Response[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [viewMode, setViewMode] = useState<ViewMode>('list');
  const [selectedItem, setSelectedItem] = useState<{ResourceName}Response | null>(null);
  const [deleteItem, setDeleteItem] = useState<{ id: number; name: string } | null>(null);

  useEffect(() => {
    fetchItems();
  }, []);

  const fetchItems = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await {resourceName}Api.getAll();
      setItems(data);
    } catch (error: any) {
      const errorMessage = error instanceof ApiError ? error.message : 'Failed to fetch';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (dto: any) => {
    await {resourceName}Api.create(dto);
    toast.success('{ResourceName} created successfully');
    setViewMode('list');
    await fetchItems();
  };

  const handleUpdate = async (dto: any) => {
    if (!selectedItem) return;
    await {resourceName}Api.update(selectedItem.id, dto);
    toast.success('{ResourceName} updated successfully');
    setViewMode('list');
    setSelectedItem(null);
    await fetchItems();
  };

  const handleDelete = async (id: number) => {
    const item = items.find(i => i.id === id);
    if (item) {
      setDeleteItem({ id, name: item.name });
    }
  };

  const handleView = (item: {ResourceName}Response) => {
    setSelectedItem(item);
    setViewMode('detail');
  };

  const handleEdit = (item: {ResourceName}Response) => {
    setSelectedItem(item);
    setViewMode('edit');
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = '/login';
  };

  return (
    <div className="{resourceName}-page">
      <header className="page-header">
        <h1>{ResourceName} Management</h1>
        <div className="header-actions">
          <button onClick={fetchItems} className="btn btn-secondary">Refresh</button>
          <button onClick={() => setViewMode('create')} className="btn btn-primary">Add New</button>
          <button onClick={handleLogout} className="btn btn-logout">Logout</button>
        </div>
      </header>

      {error && (
        <div className="error-banner">
          <span className="error-icon">❌</span>
          <span className="error-message">{error}</span>
          <button onClick={() => setError(null)} className="error-close">✕</button>
        </div>
      )}

      {viewMode === 'list' && (
        <{ResourceName}Table
          data={items}
          loading={loading}
          onRefresh={fetchItems}
          onView={handleView}
          onEdit={handleEdit}
          onDelete={handleDelete}
        />
      )}

      {viewMode === 'create' && (
        <{ResourceName}Form
          mode="create"
          onSubmit={handleCreate}
          onCancel={() => setViewMode('list')}
        />
      )}

      {viewMode === 'edit' && selectedItem && (
        <{ResourceName}Form
          mode="edit"
          initialData={selectedItem}
          onSubmit={handleUpdate}
          onCancel={() => { setViewMode('list'); setSelectedItem(null); }}
        />
      )}

      {viewMode === 'detail' && selectedItem && (
        <{ResourceName}Detail
          item={selectedItem}
          onEdit={() => setViewMode('edit')}
          onDelete={() => handleDelete(selectedItem.id)}
          onBack={() => { setViewMode('list'); setSelectedItem(null); }}
        />
      )}

      {deleteItem && (
        <{ResourceName}DeleteDialog
          item={deleteItem}
          isOpen={true}
          onClose={() => setDeleteItem(null)}
          onDeleted={() => { setDeleteItem(null); fetchItems(); }}
        />
      )}
    </div>
  );
};
```

### 8. CSS file — `{ResourceName}Page.css`

**Location:** `Frontend/src/assets/styles/{ResourceName}Page.css`

Generate CSS following the existing pattern from `LogsPage.css`:

```css
/* Page Layout */
.{resourceName}-page {
  max-width: 1400px;
  margin: 0 auto;
  padding: 24px;
  min-height: 100vh;
}

/* Header */
.page-header {
  background: white;
  padding: 20px 24px;
  border-radius: 12px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
}

.page-header h1 {
  font-size: 28px;
  font-weight: 700;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.header-actions {
  display: flex;
  gap: 12px;
}

/* Buttons */
.btn { padding: 10px 20px; border: none; border-radius: 8px; font-size: 14px; font-weight: 500; cursor: pointer; transition: all 0.3s ease; }
.btn-primary { background: #667eea; color: white; }
.btn-primary:hover { background: #5a67d8; transform: translateY(-2px); box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4); }
.btn-secondary { background: #f0f2f5; color: #1a2332; }
.btn-secondary:hover { background: #e4e7ec; transform: translateY(-2px); }
.btn-danger { background: #fee; color: #ff6b6b; }
.btn-danger:hover { background: #ff6b6b; color: white; transform: translateY(-2px); }

/* Error banner */
.error-banner { padding: 14px 20px; border-radius: 8px; margin-bottom: 20px; display: flex; align-items: center; gap: 12px; background: #fee; color: #ff6b6b; border-left: 4px solid #ff6b6b; }

/* Table */
.table-container { background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06); }
.{resourceName}-table { width: 100%; border-collapse: collapse; }
.{resourceName}-table thead { background: #f8f9fa; }
.{resourceName}-table th { padding: 16px; text-align: left; font-weight: 600; color: #6b7280; border-bottom: 2px solid #e4e7ec; }
.{resourceName}-table td { padding: 16px; border-bottom: 1px solid #f0f2f5; }
.{resourceName}-table tbody tr:hover { background: #f8f9fa; }

/* Form */
.{resourceName}-form { background: white; padding: 24px; border-radius: 12px; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06); }
.form-group { margin-bottom: 16px; }
.form-group label { display: block; margin-bottom: 6px; font-weight: 500; color: #333; }
.form-group input, .form-group select, .form-group textarea { width: 100%; padding: 12px; border: 1px solid #ddd; border-radius: 8px; font-size: 14px; }
.form-group input:focus, .form-group select:focus { outline: none; border-color: #667eea; }
.form-group .field-error { color: #ff6b6b; font-size: 12px; margin-top: 4px; }
.form-actions { display: flex; gap: 12px; justify-content: flex-end; margin-top: 24px; }

/* States */
.loading-state { padding: 60px; text-align: center; color: #6b7280; }
.empty-state { padding: 60px; text-align: center; color: #6b7280; }

/* Modal */
.modal-overlay { position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0, 0, 0, 0.5); display: flex; justify-content: center; align-items: center; z-index: 9999; }
.modal-content { background: white; border-radius: 12px; max-width: 600px; width: 90%; max-height: 80vh; overflow-y: auto; box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3); }
.modal-header { padding: 20px 24px; border-bottom: 1px solid #e4e7ec; display: flex; justify-content: space-between; align-items: center; }
.modal-body { padding: 24px; }
.modal-footer { padding: 16px 24px; border-top: 1px solid #e4e7ec; display: flex; justify-content: flex-end; gap: 12px; }

/* Responsive */
@media (max-width: 768px) {
  .{resourceName}-page { padding: 16px; }
  .page-header { flex-direction: column; gap: 16px; }
  .form-actions { flex-direction: column; }
}
```

## Component states (every component)

Every component must handle:
- **Loading**: skeleton or spinner
- **Empty** (list): friendly message with CTA
- **Error**: error message with retry button
- **Success**: normal display

## Do NOT

- Generate API client or HTTP service code (use `generate-api-client-for-controller` skill)
- Parse the controller file (use parsed metadata already provided)
- Generate test files
- Use CSS modules, Tailwind, or styled-components
- Use react-router — auth routing is conditional rendering in App.tsx
