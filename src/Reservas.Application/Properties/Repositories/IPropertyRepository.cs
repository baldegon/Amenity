using Reservas.Domain.Entities;

namespace Reservas.Application.Properties.Repositories;

public interface IPropertyRepository
{
    Task<IReadOnlyCollection<Property>> GetAllByUserAsync(int userId, CancellationToken cancellationToken = default);

    Task<Property?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default);

    Task<Property?> GetByIdForUpdateAsync(int id, int userId, CancellationToken cancellationToken = default);

    Task<Property> AddAsync(Property property, CancellationToken cancellationToken = default);

    Task<Property> UpdateAsync(Property property, CancellationToken cancellationToken = default);

    Task DeleteAsync(Property property, CancellationToken cancellationToken = default);
}
