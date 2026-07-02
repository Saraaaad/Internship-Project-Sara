public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string entity, int id)
        : base($"{entity} with ID {id} not found") { }
    public NotFoundException(string entity, string email)
        : base($"{entity} with email {email} not found") { }
}
