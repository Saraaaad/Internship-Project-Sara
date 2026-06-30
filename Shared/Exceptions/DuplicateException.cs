public class DuplicateException : Exception
{
    public DuplicateException(string message) : base(message) { }
    public DuplicateException(string entity, string value)
        : base($"{entity} with '{value}' already exists") { }
}