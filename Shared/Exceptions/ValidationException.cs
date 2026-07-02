public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
    public ValidationException(string entity, string field, string value)
        : base($"{entity} with {field} '{value}' is invalid") { }
}