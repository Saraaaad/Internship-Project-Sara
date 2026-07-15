using InternshipProjectSara.Data.Context;

public class LogService : ILogService
{
    private readonly ILogRepository logRepository;
    private readonly DatabaseContext _context;

    public LogService(ILogRepository logRepository, DatabaseContext context)
    {
        _context = context;
        this.logRepository = logRepository;
    }

    public List<Logs> GetAll()
    {
        var logs = logRepository.GetAll();
        return logs;
    }

    public List<Logs> GetAllErrors()
    {
        var logs = logRepository.GetAllErrors();
        return logs;
    }

    public List<Logs> GetByDate(DateTime date)
    {
        var logs = logRepository.GetByDate(date);
        return logs;
    }

    public List<Logs> GetByDateAndLevel(DateTime date, LogLevel level)
    {
        var logs = logRepository.GetByDateAndLevel(date, level);
        return logs;
    }

    public List<Logs> GetByLevel(LogLevel level)
    {
        var logs = logRepository.GetByLevel(level);
        return logs;
    }

    public void Log(LogLevel level, string message)
    {
        var logs = new Logs
        {
            Level = level,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };

        logRepository.Add(logs);
        _context.SaveChanges();
    }
}