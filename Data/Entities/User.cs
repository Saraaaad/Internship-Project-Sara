public class User {
    public int Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public Role Role { get; private set; }  

    public User(int id, string username, string email, string password, Role role)
    {
        Id = id;
        Username = username;
        Email = email;
        Password = password;
        Role = role;
    }

    public override string ToString()
    {
        return $"User [Id={Id}, Username={Username}, Email={Email}, Role={Role}]";
    }
}