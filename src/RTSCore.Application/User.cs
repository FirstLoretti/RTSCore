namespace RTSCore.Application;

public sealed class User
{
    public UserId Id { get; }
    public string Name { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    public User(string name, string password)
    {
        Id = UserId.New();
        Name = name;
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
    }

    private User() { }
}