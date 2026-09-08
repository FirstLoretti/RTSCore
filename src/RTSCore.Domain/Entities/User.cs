using RTSCore.Domain.Exeptions;
using RTSCore.Domain.ValueObjects;

namespace RTSCore.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public FactionType Faction { get; private set; }

    public User(string name, string passwordHash, FactionType faction)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Имя пользователя не может быть пустым.");
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Хэш пароля не может быть пустым.");
        if (faction == FactionType.None) throw new ArgumentException("Выбрана недопустимая фракция.");

        Id = Guid.NewGuid();
        Name = name.Trim();
        PasswordHash = passwordHash;
        Faction = faction;
    }

    private User() { }
}