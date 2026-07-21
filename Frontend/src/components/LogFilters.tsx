import React from 'react';

interface LogFiltersProps {
  filters: {
    view: 'all' | 'errors' | 'date' | 'level' | 'range' | 'date-level' | 'range-level';
    level: string;
    date: string;
    fromDate: string;
    toDate: string;
    fromTime: string;
    toTime: string;
  };
  onFilterChange: (filters: any) => void;
  onApplyFilters: () => void;
}

export const LogFilters: React.FC<LogFiltersProps> = ({ 
  filters, 
  onFilterChange, 
  onApplyFilters 
}) => {
  const views = [
    { value: 'all', label: 'All Logs' },
    { value: 'errors', label: 'Errors Only' },
    { value: 'date', label: 'By Date' },
    { value: 'level', label: 'By Level' },
    { value: 'date-level', label: 'Date + Level' },
    { value: 'range', label: 'Date Range' },
    { value: 'range-level', label: 'Range + Level' },
  ];

  const logLevels = ['Information', 'Warning', 'Error', 'Debug', 'Trace', 'Critical'];

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    onFilterChange({ ...filters, [name]: value });
  };

  const handleViewChange = (view: typeof filters.view) => {
    onFilterChange({ ...filters, view, level: '', date: '', fromDate: '', toDate: '', fromTime: '', toTime: '' });
  };

  return (
    <div className="filters-container">
      <div className="filter-section">
        <div className="view-buttons">
          {views.map(view => (
            <button
              key={view.value}
              className={`btn-view ${filters.view === view.value ? 'active' : ''}`}
              onClick={() => handleViewChange(view.value as any)}
            >
              {view.label}
            </button>
          ))}
        </div>

        <div className="filter-controls">
          {(filters.view === 'date' || filters.view === 'date-level') && (
            <input
              type="date"
              name="date"
              value={filters.date}
              onChange={handleChange}
              className="filter-input"
              placeholder="Select date"
            />
          )}

          {(filters.view === 'level' || filters.view === 'date-level' || filters.view === 'range-level') && (
            <select
              name="level"
              value={filters.level}
              onChange={handleChange}
              className="filter-input"
            >
              <option value="">Select Level</option>
              {logLevels.map(level => (
                <option key={level} value={level}>{level}</option>
              ))}
            </select>
          )}

          {(filters.view === 'range' || filters.view === 'range-level') && (
            <div className="date-range-group">
              <div className="date-input-group">
                <label>From</label>
                <input
                  type="date"
                  name="fromDate"
                  value={filters.fromDate}
                  onChange={handleChange}
                  className="filter-input"
                />
                <input
                  type="time"
                  name="fromTime"
                  value={filters.fromTime}
                  onChange={handleChange}
                  className="filter-input time-input"
                />
              </div>
              <div className="date-input-group">
                <label>To</label>
                <input
                  type="date"
                  name="toDate"
                  value={filters.toDate}
                  onChange={handleChange}
                  className="filter-input"
                />
                <input
                  type="time"
                  name="toTime"
                  value={filters.toTime}
                  onChange={handleChange}
                  className="filter-input time-input"
                />
              </div>
            </div>
          )}

          <button onClick={onApplyFilters} className="btn btn-primary">
            Apply Filters
          </button>
        </div>
      </div>
    </div>
  );
};