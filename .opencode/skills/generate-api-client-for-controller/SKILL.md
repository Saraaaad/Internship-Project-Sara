---
name: generate-api-client-for-controller
description: Generate a typed HTTP client service from parsed controller metadata. Creates a TypeScript fetch-based API module with typed methods for each controller action. Use ONLY when generating API client code. Do NOT generate UI components.
---

# Generate API Client for Controller

Generate a typed HTTP client service matching every action in the controller.

## Frontend Stack

- **React 19** + **TypeScript** + **Vite**
- **No axios** — use native `fetch` API
- **No router** — conditional rendering based on auth state
- **sonner** for toast notifications (import `toast` from `sonner`)
- Auth token stored in `localStorage` under key `token`

## Input

You receive parsed controller metadata (from `parse-csharp-controller`):
- All actions with HTTP method, full route, parameter bindings, and response types
- All request DTOs as TypeScript interfaces
- All response DTOs as TypeScript interfaces
- All enums as TypeScript union types

## What to generate

### 1. TypeScript type definitions file

**Location:** `Frontend/src/types/{resourceName}.types.ts`

For each DTO referenced by the controller:
- Generate an interface with camelCase properties (ASP.NET Core serialization convention)
- Map C# types:
  - `string` → `string`
  - `int`, `long` → `number`
  - `decimal`, `double`, `float` → `number`
  - `bool` → `boolean`
  - `DateTime`, `DateTimeOffset` → `string` (ISO 8601)
  - `Guid` → `string`
  - `Enum` → union type of string literals
  - `List<T>` → `T[]`
  - `Dictionary<K,V>` → `Record<K, V>`
  - `T?` → `T | null`

For each enum:
```typescript
export type Role = 'Admin' | 'HR' | 'Lead' | 'Employee';
```

Add helper functions if the domain needs them (e.g., enum display names, badge mappings).

### 2. API client module

**Location:** `Frontend/src/api/{resourceName}Api.ts`

Generate a module that follows this exact structure:

```typescript
import type { ResponseDto, RequestDto } from '../types/{resourceName}.types';

const API_BASE_URL = "http://localhost:5000/api";

const getAuthToken = (): string | null => {
  return localStorage.getItem("token");
};

const getRequestOptions = (method: string, body?: any): RequestInit => {
  const token = getAuthToken();

  const headers: HeadersInit = {
    "Content-Type": "application/json",
  };

  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  const options: RequestInit = {
    method,
    headers,
  };

  if (body) {
    options.body = JSON.stringify(body);
  }

  return options;
};

export class ApiError extends Error {
  status: number;
  details?: any;

  constructor(status: number, message: string, details?: any) {
    super(message);
    this.status = status;
    this.details = details;
    this.name = "ApiError";
    Object.setPrototypeOf(this, ApiError.prototype);
  }
}

const handleResponse = async <T>(response: Response): Promise<T> => {
  if (response.status === 401) {
    localStorage.removeItem("token");
    window.location.href = "/login";
    throw new ApiError(401, "Session expired. Please login again.");
  }

  if (response.status === 400) {
    let errorMessage = "Validation error";
    try {
      const errorData = await response.text();
      if (errorData) {
        errorMessage = errorData;
      }
    } catch (e) {}
    throw new ApiError(400, errorMessage);
  }

  if (!response.ok) {
    let errorMessage = `HTTP error! status: ${response.status}`;
    try {
      const errorData = await response.text();
      if (errorData) {
        errorMessage = errorData;
      }
    } catch (e) {}
    throw new ApiError(response.status, errorMessage);
  }

  const contentLength = response.headers.get("content-length");
  if (contentLength === "0") {
    return {} as T;
  }

  try {
    return await response.json();
  } catch {
    return {} as T;
  }
};

export const {resourceName}Api = {
  // One method per controller action
  getAll: async (): Promise<ResponseType[]> => {
    const response = await fetch(
      `${API_BASE_URL}/{routePrefix}`,
      getRequestOptions("GET"),
    );
    return handleResponse<ResponseType[]>(response);
  },

  getById: async (id: number): Promise<ResponseType> => {
    const response = await fetch(
      `${API_BASE_URL}/{routePrefix}/${id}`,
      getRequestOptions("GET"),
    );
    return handleResponse<ResponseType>(response);
  },

  create: async (dto: RequestType): Promise<ResponseType> => {
    const response = await fetch(
      `${API_BASE_URL}/{routePrefix}`,
      getRequestOptions("POST", dto),
    );
    return handleResponse<ResponseType>(response);
  },

  update: async (id: number, dto: RequestType): Promise<ResponseType> => {
    const response = await fetch(
      `${API_BASE_URL}/{routePrefix}/${id}`,
      getRequestOptions("PUT", dto),
    );
    return handleResponse<ResponseType>(response);
  },

  delete: async (id: number): Promise<void> => {
    const response = await fetch(
      `${API_BASE_URL}/{routePrefix}/${id}`,
      getRequestOptions("DELETE"),
    );
    return handleResponse<void>(response);
  },
};
```

### 3. Method naming conventions

| Controller Action | API Method Name | HTTP Method | Route Pattern |
|---|---|---|---|
| `GetAll` | `getAll` | GET | `{routePrefix}` |
| `GetById` | `getById` | GET | `{routePrefix}/{id}` |
| `Create` | `create` | POST | `{routePrefix}` |
| `Update` | `update` | PUT | `{routePrefix}/{id}` |
| `Delete` | `delete` | DELETE | `{routePrefix}/{id}` |
| Custom actions | camelCase version of action name | From attribute | Full route from attribute |

### 4. Error handling

The `ApiError` class is already provided in the template. Consumers handle errors like:
```typescript
try {
  const data = await userApi.getAll();
} catch (error: any) {
  const errorMessage = error instanceof ApiError ? error.message : 'Failed to fetch';
}
```

## Location

- Types: `Frontend/src/types/{resourceName}.types.ts`
- API client: `Frontend/src/api/{resourceName}Api.ts`

## Do NOT

- Generate UI components (forms, tables, pages)
- Use axios or any HTTP library — use native fetch
- Use react-router — auth routing is conditional rendering
- Parse the controller file (use parsed metadata already provided)
- Generate class-based services — use object literals with exported functions
