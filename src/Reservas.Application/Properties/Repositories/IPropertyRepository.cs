using Reservas.Domain.Entities;

namespace Reservas.Application.Properties.Repositories;

public interface IPropertyRepository
{
    Task<IReadOnlyCollection<Property>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Property?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Property> AddAsync(Property property, CancellationToken cancellationToken = default);
}
