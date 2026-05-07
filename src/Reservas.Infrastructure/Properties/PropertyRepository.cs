using Microsoft.EntityFrameworkCore;
using Reservas.Application.Properties.Repositories;
using Reservas.Domain.Entities;
using Reservas.Infrastructure.Data;

namespace Reservas.Infrastructure.Properties;

public class PropertyRepository : IPropertyRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PropertyRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Property>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Properties
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Property?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(property => property.Id == id, cancellationToken);
    }

    public async Task<Property> AddAsync(Property property, CancellationToken cancellationToken = default)
    {
        _dbContext.Properties.Add(property);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return property;
    }
}
