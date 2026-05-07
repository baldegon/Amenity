using Reservas.Domain.Entities;

namespace Reservas.Application.Auth.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
}
