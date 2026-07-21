import React, { useState } from 'react';
import { type LogEntry, getLevelName, getLevelBadge } from '../types/log.types';

interface LogTableProps {
  logs: LogEntry[];
  loading: boolean;
  onRefresh: () => void;
  selectedLogs: number[];
  onSelectLog: (id: number) => void;
  onSelectAll: () => void;
  onDeleteSelected: () => void;
}

export const LogTable: React.FC<LogTableProps> = ({ 
  logs, 
  loading, 
  onRefresh,
  selectedLogs,
  onSelectLog,
  onSelectAll,
  onDeleteSelected
}) => {
  const [expandedId, setExpandedId] = useState<number | null>(null);
  const [viewingLog, setViewingLog] = useState<LogEntry | null>(null);

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleString();
  };

  const handleViewLog = (log: LogEntry) => {
    setViewingLog(log);
  };

  const closeViewModal = () => {
    setViewingLog(null);
  };

  if (loading) {
    return <div className="loading-state">Loading logs...</div>;
  }

  if (logs.length === 0) {
    return (
      <div className="empty-state">
        <p>No logs found</p>
        <button onClick={onRefresh} className="btn btn-primary">Refresh</button>
      </div>
    );
  }

  const allSelected = logs.length > 0 && logs.every(log => selectedLogs.includes(log.id));

  return (
    <>
      <div className="table-container">
        <div className="table-toolbar">
          {selectedLogs.length > 0 && (
            <button 
              onClick={onDeleteSelected} 
              className="btn btn-danger"
            >
              🗑️ Delete Selected ({selectedLogs.length})
            </button>
          )}
        </div>

        <table className="log-table">
          <thead>
            <tr>
              <th style={{ width: '40px' }}>
                <input
                  type="checkbox"
                  checked={allSelected}
                  onChange={onSelectAll}
                />
              </th>
              <th>Level</th>
              <th>Message</th>
              <th>Created At</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {logs.map((log) => (
              <tr key={log.id} className={selectedLogs.includes(log.id) ? 'selected' : ''}>
                <td>
                  <input
                    type="checkbox"
                    checked={selectedLogs.includes(log.id)}
                    onChange={() => onSelectLog(log.id)}
                  />
                </td>
                <td>
                  <span className={`badge ${getLevelBadge(log.level)}`}>
                    {getLevelName(log.level)}
                  </span>
                </td>
                <td className="log-message">
                  {log.message && log.message.length > 100 ? (
                    <>
                      {expandedId === log.id ? log.message : `${log.message.substring(0, 100)}...`}
                      <button 
                        className="expand-btn"
                        onClick={() => setExpandedId(expandedId === log.id ? null : log.id)}
                      >
                        {expandedId === log.id ? 'Show less' : 'Show more'}
                      </button>
                    </>
                  ) : (
                    log.message || 'No message'
                  )}
                </td>
                <td>{formatDate(log.createdAt)}</td>
                <td>
                  <button 
                    className="btn btn-detail" 
                    onClick={() => handleViewLog(log)}
                  >
                    View
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* View Log Modal */}
      {viewingLog && (
        <div className="modal-overlay" onClick={closeViewModal}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>Log Details</h2>
              <button className="modal-close" onClick={closeViewModal}>✕</button>
            </div>
            <div className="modal-body">
              <div className="detail-row">
                <label>ID:</label>
                <span>{viewingLog.id}</span>
              </div>
              <div className="detail-row">
                <label>Level:</label>
                <span className={`badge ${getLevelBadge(viewingLog.level)}`}>
                  {getLevelName(viewingLog.level)}
                </span>
              </div>
              <div className="detail-row">
                <label>Message:</label>
                <pre>{viewingLog.message}</pre>
              </div>
              <div className="detail-row">
                <label>Created At:</label>
                <span>{formatDate(viewingLog.createdAt)}</span>
              </div>
            </div>
          </div>
        </div>
      )}
    </>
  );
};