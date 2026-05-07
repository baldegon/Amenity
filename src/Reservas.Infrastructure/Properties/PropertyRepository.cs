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

    public async Task<IReadOnlyCollection<Property>> GetAllByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Properties
            .AsNoTracking()
            .Where(property => property.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Property?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(property => property.Id == id && property.UserId == userId, cancellationToken);
    }

    public async Task<Property?> GetByIdForUpdateAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Properties
            .FirstOrDefaultAsync(property => property.Id == id && property.UserId == userId, cancellationToken);
    }

    public async Task<Property> AddAsync(Property property, CancellationToken cancellationToken = default)
    {
        _dbContext.Properties.Add(property);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return property;
    }

    public async Task<Property> UpdateAsync(Property property, CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
        return property;
    }

    public async Task DeleteAsync(Property property, CancellationToken cancellationToken = default)
    {
        _dbContext.Properties.Remove(property);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
