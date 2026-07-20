using InternshipProjectSara.Data.Context;

public class LogService : ILogService
{
    private readonly ILogRepository logRepository;
    private readonly DatabaseContext _context;
    private readonly ILogger<LogService> logger;

    public LogService(ILogRepository logRepository, DatabaseContext context, ILogger<LogService> logger)
    {
        _context = context;
        this.logRepository = logRepository;
        this.logger = logger;
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
        if (date.Date > DateTime.UtcNow.Date)
        {
            logger.LogWarning("Failed, Date must be in the past");
            throw new ValidationException("Date must be in the past");
        }
        var logs = logRepository.GetByDate(date);
        logger.LogInformation("Logs retrieved by date successfully");
        return logs;
    }

    public List<Logs> GetByDateAndLevel(DateTime date, LogLevel level)
    {
        if (date.Date > DateTime.UtcNow.Date)
        {
            logger.LogWarning("Failed, Date must be in the past");
            throw new ValidationException("Date must be in the past");
        }
        if (!Enum.IsDefined(typeof(LogLevel), level))
        {
            logger.LogWarning("Failed, Invalid log level. Valid levels are: " + string.Join(", ", Enum.GetNames(typeof(LogLevel))));
            throw new ValidationException("Invalid log level. Valid levels are: " + string.Join(", ", Enum.GetNames(typeof(LogLevel))));
        }

        var logs = logRepository.GetByDateAndLevel(date, level);
        logger.LogInformation("Logs retrieved by level and date successfully");
        return logs;
    }

    public List<Logs> GetByLevel(LogLevel level)
    {
        if (!Enum.IsDefined(typeof(LogLevel), level))
        {
            logger.LogWarning("Failed, Invalid log level. Valid levels are: " + string.Join(", ", Enum.GetNames(typeof(LogLevel))));
            throw new ValidationException("Invalid log level. Valid levels are: " + string.Join(", ", Enum.GetNames(typeof(LogLevel))));
        }
        var logs = logRepository.GetByLevel(level);
        logger.LogInformation("Logs retrieved by level successfully");
        return logs;
    }

    public void Log(LogLevel level, string message)
    {
        if (!Enum.IsDefined(typeof(LogLevel), level))
        {
            logger.LogWarning("Failed, Invalid log level. Valid levels are: " + string.Join(", ", Enum.GetNames(typeof(LogLevel))));
            throw new ValidationException("Invalid log level. Valid levels are: " + string.Join(", ", Enum.GetNames(typeof(LogLevel))));
        }
        if (string.IsNullOrEmpty(message))
        {
            logger.LogWarning("Failed, Message cannot be null or empty");
            throw new ValidationException("Message cannot be null or empty");
        }
        var logs = new Logs
        {
            Level = level,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };

        logRepository.Add(logs);
        _context.SaveChanges();
    }

    public void Clear(int lastDays)
    {
        if (lastDays < 0)
        {
            logger.LogWarning("Failed, Last days must be greater than 0");
            throw new ValidationException("Last days must be greater than 0");
        }
        logRepository.Clear(lastDays);
        _context.SaveChanges();
    }
    public List<Logs> GetByDateRange(DateTime from, DateTime to)
    {
        if (from.Date > DateTime.UtcNow.Date || to.Date > DateTime.UtcNow.Date)
        {
            logger.LogWarning("Failed, Date must be in the past");
            throw new ValidationException("Date must be in the past");
        }
        if (from.Date > to.Date)
        {
            logger.LogWarning("Failed, From date must be less than to date");
            throw new ValidationException("From date must be less than to date");
        }

        var fromDate = from.Date;
        var toDate = to.Date.AddDays(1).AddTicks(-1);

        var logs = logRepository.GetByDateRange(from, to);
        return logs;
    }

    public void DeleteLog(int id)
    {
        var log = logRepository.GetById(id);
        if (id <= 0)
        {
            logger.LogWarning("Failed, Id must be greater than 0");
            throw new ValidationException("Id must be greater than 0");
        }
        if (log == null)
        {
            logger.LogWarning("Log not found");
            throw new ValidationException("Log not found");
        }
        logRepository.Delete(id);
        _context.SaveChanges();
        logger.LogInformation("Log deleted successfully");
    }

    public List<Logs> GetByDateRangeAndLevel(DateTime from, DateTime to, LogLevel level)
    {
        if (from.Date > DateTime.UtcNow.Date || to.Date > DateTime.UtcNow.Date)
        {
            logger.LogWarning("Failed, Date must be in the past or today");
            throw new ValidationException("Date must be in the past or today");
        }
        if (from.Date > to.Date)
        {
            logger.LogWarning("Failed, From date must be less than to date");
            throw new ValidationException("From date must be less than to date");
        }
        if (!Enum.IsDefined(typeof(LogLevel), level))
        {
            logger.LogWarning("Failed, Invalid log level. Valid levels are: " + string.Join(", ", Enum.GetNames(typeof(LogLevel))));
            throw new ValidationException("Invalid log level. Valid levels are: " + string.Join(", ", Enum.GetNames(typeof(LogLevel))));
        }
        var fromDate = from.Date;
        var toDate = to.Date.AddDays(1).AddTicks(-1);

        var logs = logRepository.GetByDateRangeAndLevel(from, to, level);
        return logs;
    }
}