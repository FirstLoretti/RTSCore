namespace RTSCore.Application.Repositories;

public interface IUserRepository
{
    void Add(User user);

    Task<bool> ExistAsync(string name, CancellationToken ct);
    Task<User?> GetAsync(string name, CancellationToken ct);
    Task<User?> GetAsync(UserId id, CancellationToken ct);
}