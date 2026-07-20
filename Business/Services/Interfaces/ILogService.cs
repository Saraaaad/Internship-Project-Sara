public interface ILogService
{
    void Log(LogLevel level, string message);
    List<Logs> GetAll();
    List<Logs> GetByLevel(LogLevel level);
    List<Logs> GetByDate(DateTime date);
    List<Logs> GetByDateAndLevel(DateTime date, LogLevel level);
    List<Logs> GetAllErrors();
    void Clear(int lastDays);
    List<Logs> GetByDateRange(DateTime from, DateTime to);
    void DeleteLog(int id);
    List<Logs> GetByDateRangeAndLevel(DateTime from, DateTime to, LogLevel level);
}