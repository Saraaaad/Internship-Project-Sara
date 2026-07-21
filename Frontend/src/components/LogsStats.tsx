import React from 'react';
import type { LogStats as LogStatsType } from '../types/log.types';

interface LogStatsProps {
  stats: LogStatsType;
}

export const LogStats: React.FC<LogStatsProps> = ({ stats }) => {
  const statCards = [
    { label: 'Total Logs', value: stats.total, icon: '📊', color: '#667eea' },
    { label: 'Errors', value: stats.errors, icon: '❌', color: '#ff6b6b' },
    { label: 'Warnings', value: stats.warnings, icon: '⚠️', color: '#feca57' },
    { label: 'Information', value: stats.info, icon: 'ℹ️', color: '#4a9eff' },
  ];

  return (
    <div className="stats-grid">
      {statCards.map((stat, index) => (
        <div key={index} className="stat-card" style={{ borderLeftColor: stat.color }}>
          <div className="stat-icon">{stat.icon}</div>
          <div className="stat-content">
            <div className="stat-label">{stat.label}</div>
            <div className="stat-value">{stat.value}</div>
          </div>
        </div>
      ))}
    </div>
  );
};