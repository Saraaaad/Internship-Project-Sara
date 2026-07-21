import React, { useState, useEffect } from 'react';
import { LogStats } from '../components/LogsStats';
import { LogTable } from '../components/LogTable';
import { LogFilters } from '../components/LogFilters';
import { logsApi, ApiError } from '../api/logsApi';
import { type LogEntry, type LogStats as LogStatsType, getLevelName, isErrorLevel } from '../types/log.types';
import '../assets/styles/LogsPage.css';

export const LogsPage: React.FC = () => {
  const [logs, setLogs] = useState<LogEntry[]>([]);
  const [filteredLogs, setFilteredLogs] = useState<LogEntry[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [selectedLogs, setSelectedLogs] = useState<number[]>([]);
  const [stats, setStats] = useState<LogStatsType>({ total: 0, errors: 0, warnings: 0, info: 0 });
  const [filters, setFilters] = useState({
    view: 'all' as 'all' | 'errors' | 'date' | 'level' | 'range' | 'date-level' | 'range-level',
    level: '',
    date: '',
    fromDate: '',
    toDate: '',
    fromTime: '',
    toTime: '',
  });
  const [hasFilterError, setHasFilterError] = useState(false);

  useEffect(() => {
    fetchLogs();
  }, []);

  const fetchLogs = async () => {
    setLoading(true);
    setError(null);
    setHasFilterError(false);
    try {
      const data = await logsApi.getAll();
      setLogs(data);
      setFilteredLogs(data);
      calculateStats(data);
      setSelectedLogs([]);
    } catch (error: any) {
      console.error('Fetch error:', error);
      const errorMessage = error instanceof ApiError ? error.message : 'Failed to fetch logs';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const calculateStats = (logsData: LogEntry[]) => {
    setStats({
      total: logsData.length,
      errors: logsData.filter(log => isErrorLevel(log.level)).length,
      warnings: logsData.filter(log => getLevelName(log.level) === 'Warning').length,
      info: logsData.filter(log => getLevelName(log.level) === 'Information' || getLevelName(log.level) === 'Debug').length,
    });
  };

  const handleFilter = async () => {
    setLoading(true);
    setError(null);
    setHasFilterError(false);
    try {
      let data;
      let from, to;

      switch (filters.view) {
        case 'errors':
          data = await logsApi.getErrors();
          break;
          
        case 'date':
          if (filters.date) {
            data = await logsApi.getByDate(filters.date);
          } else {
            data = await logsApi.getAll();
          }
          break;
          
        case 'level':
          if (filters.level) {
            data = await logsApi.getByLevel(filters.level as any);
          } else {
            data = await logsApi.getAll();
          }
          break;
          
        case 'date-level':
          if (filters.date && filters.level) {
            data = await logsApi.getByDateAndLevel(filters.date, filters.level as any);
          } else if (filters.date) {
            data = await logsApi.getByDate(filters.date);
          } else if (filters.level) {
            data = await logsApi.getByLevel(filters.level as any);
          } else {
            data = await logsApi.getAll();
          }
          break;
          
        case 'range':
          if (filters.fromDate && filters.toDate) {
            from = filters.fromDate;
            to = filters.toDate;
            if (filters.fromTime) from = `${from}T${filters.fromTime}:00`;
            if (filters.toTime) to = `${to}T${filters.toTime}:59`;
            data = await logsApi.getByDateRange(from, to);
          } else {
            data = await logsApi.getAll();
          }
          break;
          
        case 'range-level':
          if (filters.fromDate && filters.toDate && filters.level) {
            from = filters.fromDate;
            to = filters.toDate;
            if (filters.fromTime) from = `${from}T${filters.fromTime}:00`;
            if (filters.toTime) to = `${to}T${filters.toTime}:59`;
            data = await logsApi.getByDateRangeAndLevel(from, to, filters.level as any);
          } else if (filters.fromDate && filters.toDate) {
            from = filters.fromDate;
            to = filters.toDate;
            if (filters.fromTime) from = `${from}T${filters.fromTime}:00`;
            if (filters.toTime) to = `${to}T${filters.toTime}:59`;
            data = await logsApi.getByDateRange(from, to);
          } else if (filters.level) {
            data = await logsApi.getByLevel(filters.level as any);
          } else {
            data = await logsApi.getAll();
          }
          break;
          
        default:
          data = await logsApi.getAll();
      }
      
      // Important: Set filtered logs even if empty
      setFilteredLogs(data);
      calculateStats(data);
      setSelectedLogs([]);
      
      // Show info message if no logs found
      if (data.length === 0) {
        setError('No logs found matching your filters');
        setHasFilterError(true);
      }
      
    } catch (error: any) {
      console.error('Filter error:', error);
      // Extract meaningful error message
      let errorMessage = 'Failed to apply filters';
      if (error instanceof ApiError) {
        errorMessage = error.message;
        if (errorMessage.includes('Date must be in the past')) {
          errorMessage = '⚠️ Please select a date in the past (not future)';
        }
      }
      setError(errorMessage);
      setHasFilterError(true);
      // Reset to show all logs on filter error
      try {
        const allData = await logsApi.getAll();
        setFilteredLogs(allData);
        calculateStats(allData);
      } catch (e) {
        // If we can't fetch all logs, keep current state
      }
    } finally {
      setLoading(false);
    }
  };

  const handleClearLogs = async (days: number) => {
    if (!window.confirm(`Delete logs older than ${days} days?`)) return;
    
    setLoading(true);
    setError(null);
    try {
      await logsApi.clearLogs(days);
      alert(`Logs older than ${days} days cleared successfully!`);
      await fetchLogs();
    } catch (error: any) {
      console.error('Clear error:', error);
      const errorMessage = error instanceof ApiError ? error.message : 'Failed to clear logs';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteSelected = async () => {
    if (selectedLogs.length === 0) {
      alert('Please select logs to delete');
      return;
    }

    if (!window.confirm(`Delete ${selectedLogs.length} selected log(s)?`)) return;

    setLoading(true);
    setError(null);
    try {
      for (const id of selectedLogs) {
        await logsApi.deleteLog(id);
      }
      alert(`${selectedLogs.length} log(s) deleted successfully!`);
      await fetchLogs();
    } catch (error: any) {
      console.error('Delete error:', error);
      const errorMessage = error instanceof ApiError ? error.message : 'Failed to delete logs';
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const handleSelectLog = (id: number) => {
    setSelectedLogs(prev => 
      prev.includes(id) 
        ? prev.filter(logId => logId !== id)
        : [...prev, id]
    );
  };

  const handleSelectAll = () => {
    if (selectedLogs.length === filteredLogs.length) {
      setSelectedLogs([]);
    } else {
      setSelectedLogs(filteredLogs.map(log => log.id));
    }
  };

  const handleRefresh = () => {
    fetchLogs();
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = '/login';
  };

  const clearError = () => {
    setError(null);
    setHasFilterError(false);
  };

  return (
    <div className="logs-page">
      <header className="page-header">
        <h1>📊 Log Management</h1>
        <div className="header-actions">
          <button onClick={handleRefresh} className="btn btn-secondary">
            🔄 Refresh
          </button>
          <button onClick={() => handleClearLogs(30)} className="btn btn-danger">
            🗑️ Clear (30 days)
          </button>
          <button onClick={handleLogout} className="btn btn-logout">
            🚪 Logout
          </button>
        </div>
      </header>

      {error && (
        <div className={`error-banner ${hasFilterError ? 'error-warning' : ''}`}>
          <span className="error-icon">{hasFilterError ? '⚠️' : '❌'}</span>
          <span className="error-message">{error}</span>
          <button onClick={clearError} className="error-close">✕</button>
          {hasFilterError && (
            <button onClick={() => {
              setError(null);
              setHasFilterError(false);
              // Reload all logs
              fetchLogs();
            }} className="error-reset">
              Reset Filters
            </button>
          )}
        </div>
      )}

      <LogStats stats={stats} />

      <LogFilters 
        filters={filters}
        onFilterChange={setFilters}
        onApplyFilters={handleFilter}
      />

      <LogTable 
        logs={filteredLogs} 
        loading={loading}
        onRefresh={handleRefresh}
        selectedLogs={selectedLogs}
        onSelectLog={handleSelectLog}
        onSelectAll={handleSelectAll}
        onDeleteSelected={handleDeleteSelected}
      />
    </div>
  );
};