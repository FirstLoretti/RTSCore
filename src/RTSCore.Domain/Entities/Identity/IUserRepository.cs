namespace RTSCore.Domain.Entities.Identity;

public interface IUserRepository
{
    void Add(User user);

    Task<bool> ExistAsync(string name, CancellationToken cancellationToken);
    Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}