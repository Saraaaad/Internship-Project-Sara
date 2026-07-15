using InternshipProjectSara.Data.Context;

public class LogRepository : Repository<Logs>, ILogRepository
{
    public LogRepository(DatabaseContext context) : base(context) { }
    public List<Logs> GetAllErrors()
    {
        return _dbSet.Where(l => l.Level == LogLevel.Error || l.Level == LogLevel.Critical).ToList();
    }

    public List<Logs> GetByDate(DateTime date)
    {
        return _dbSet.Where(l => l.CreatedAt.Date == date.Date).ToList();
    }

    public List<Logs> GetByDateAndLevel(DateTime date, LogLevel level)
    {
        return _dbSet.Where(l => l.CreatedAt.Date == date.Date && l.Level == level).ToList();
    }
    public List<Logs> GetByLevel(LogLevel level)
    {
        return _dbSet.Where(l => l.Level == level).ToList();
    }
}