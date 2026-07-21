export type LogLevel = 'Information' | 'Warning' | 'Error' | 'Debug' | 'Trace' | 'Critical';

export interface LogEntry {
  id: number;
  level: LogLevel | number;  // Allow both string and number
  message: string;
  createdAt: string;
}

export interface LogStats {
  total: number;
  errors: number;
  warnings: number;
  info: number;
}

// Helper to convert numeric level to string
export const getLevelName = (level: LogLevel | number): string => {
  const levelMap: Record<number, LogLevel> = {
    0: 'Information',
    1: 'Warning',
    2: 'Error',
    3: 'Debug',
    4: 'Trace',
    5: 'Critical'
  };
  
  if (typeof level === 'number') {
    return levelMap[level] || 'Information';
  }
  return level;
};

// Helper to get badge class
export const getLevelBadge = (level: LogLevel | number): string => {
  const levelName = getLevelName(level);
  const badges: Record<string, string> = {
    'Information': 'badge-info',
    'Warning': 'badge-warning',
    'Error': 'badge-error',
    'Critical': 'badge-critical',
    'Debug': 'badge-debug',
    'Trace': 'badge-trace',
  };
  return badges[levelName] || 'badge-default';
};

// Helper to check if level is error
export const isErrorLevel = (level: LogLevel | number): boolean => {
  const levelName = getLevelName(level);
  return levelName === 'Error' || levelName === 'Critical';
};