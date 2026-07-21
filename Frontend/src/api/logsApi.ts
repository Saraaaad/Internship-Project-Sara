import type { LogEntry, LogLevel } from "../types/log.types";

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

// Improved error handling with specific error types
export class ApiError extends Error {
  status: number;
  details?: any;

  constructor(status: number, message: string, details?: any) {
    super(message);
    this.status = status;
    this.details = details;
    this.name = "ApiError";

    // This is needed for proper prototype chain in TypeScript
    Object.setPrototypeOf(this, ApiError.prototype);
  }
}

const handleResponse = async <T>(response: Response): Promise<T> => {
  // Handle 401 Unauthorized
  if (response.status === 401) {
    localStorage.removeItem("token");
    window.location.href = "/login";
    throw new ApiError(401, "Session expired. Please login again.");
  }

  // Handle 400 Bad Request (Validation errors)
  if (response.status === 400) {
    let errorMessage = "Validation error";
    try {
      const errorData = await response.text();
      if (errorData) {
        errorMessage = errorData;
      }
    } catch (e) {
      // If we can't parse the response, use default message
    }
    throw new ApiError(400, errorMessage);
  }

  // Handle other error status codes
  if (!response.ok) {
    let errorMessage = `HTTP error! status: ${response.status}`;
    try {
      const errorData = await response.text();
      if (errorData) {
        errorMessage = errorData;
      }
    } catch (e) {
      // Use default message
    }
    throw new ApiError(response.status, errorMessage);
  }

  // Handle empty responses
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

export const logsApi = {
  getAll: async (): Promise<LogEntry[]> => {
    const response = await fetch(
      `${API_BASE_URL}/logs`,
      getRequestOptions("GET"),
    );
    return handleResponse<LogEntry[]>(response);
  },

  getErrors: async (): Promise<LogEntry[]> => {
    const response = await fetch(
      `${API_BASE_URL}/logs/errors`,
      getRequestOptions("GET"),
    );
    return handleResponse<LogEntry[]>(response);
  },

  getByDate: async (date: string): Promise<LogEntry[]> => {
    const response = await fetch(
      `${API_BASE_URL}/logs/date/${date}`,
      getRequestOptions("GET"),
    );
    return handleResponse<LogEntry[]>(response);
  },

  getByLevel: async (level: LogLevel): Promise<LogEntry[]> => {
    const response = await fetch(
      `${API_BASE_URL}/logs/level/${level}`,
      getRequestOptions("GET"),
    );
    return handleResponse<LogEntry[]>(response);
  },

  getByDateAndLevel: async (
    date: string,
    level: LogLevel,
  ): Promise<LogEntry[]> => {
    const response = await fetch(
      `${API_BASE_URL}/logs/date-level/${date}/${level}`,
      getRequestOptions("GET"),
    );
    return handleResponse<LogEntry[]>(response);
  },

  getByDateRange: async (from: string, to: string): Promise<LogEntry[]> => {
    const response = await fetch(
      `${API_BASE_URL}/logs/date-range/${from}/${to}`,
      getRequestOptions("GET"),
    );
    return handleResponse<LogEntry[]>(response);
  },

  getByDateRangeAndLevel: async (
    from: string,
    to: string,
    level: LogLevel,
  ): Promise<LogEntry[]> => {
    const response = await fetch(
      `${API_BASE_URL}/logs/date-range-level/${from}/${to}/${level}`,
      getRequestOptions("GET"),
    );
    return handleResponse<LogEntry[]>(response);
  },

  clearLogs: async (days: number): Promise<void> => {
    const response = await fetch(
      `${API_BASE_URL}/logs/clear/${days}`,
      getRequestOptions("DELETE"),
    );
    return handleResponse<void>(response);
  },

  deleteLog: async (id: number): Promise<void> => {
    const response = await fetch(
      `${API_BASE_URL}/logs/${id}`,
      getRequestOptions("DELETE"),
    );
    return handleResponse<void>(response);
  },
};
