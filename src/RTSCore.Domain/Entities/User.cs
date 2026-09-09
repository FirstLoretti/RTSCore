namespace RTSCore.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    public User(string name, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Имя пользователя не может быть пустым.");
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Хэш пароля не может быть пустым.");

        Id = Guid.NewGuid();
        Name = name.Trim();
        PasswordHash = passwordHash;
    }

    private User() { }
}