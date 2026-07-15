public interface ILogRepository: IRepository<Logs>
{
    List<Logs> GetByLevel(LogLevel level);
    List<Logs> GetAllErrors();
    List<Logs> GetByDate(DateTime date);
    List<Logs> GetByDateAndLevel(DateTime date, LogLevel level);}