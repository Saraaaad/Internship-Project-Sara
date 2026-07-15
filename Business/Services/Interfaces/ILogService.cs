public interface ILogService
{
    void Log(LogLevel level, string message);
    List<Logs> GetAll();
    List<Logs> GetByLevel(LogLevel level);
    List<Logs> GetByDate(DateTime date);
    List<Logs> GetByDateAndLevel(DateTime date, LogLevel level);
    List<Logs> GetAllErrors();


}