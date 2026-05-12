using Microsoft.EntityFrameworkCore;
using Reservas.Application.Reservations.Repositories;
using Reservas.Domain.Entities;
using Reservas.Infrastructure.Data;

namespace Reservas.Infrastructure.Reservations;

public class ReservationRepository : IReservationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ReservationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Reservation>> GetByPropertyAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reservations
            .AsNoTracking()
            .Where(reservation => reservation.PropertyId == propertyId)
            .OrderBy(reservation => reservation.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Reservation?> GetByIdAsync(int reservationId, int propertyId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reservations
            .FirstOrDefaultAsync(reservation => reservation.Id == reservationId && reservation.PropertyId == propertyId, cancellationToken);
    }

    public async Task<bool> HasOverlappingReservationAsync(int propertyId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reservations
            .AsNoTracking()
            .AnyAsync(
                reservation => reservation.PropertyId == propertyId
                    && reservation.Status == ReservationStatus.Active
                    && reservation.StartDate < endDate
                    && startDate < reservation.EndDate,
                cancellationToken);
    }

    public async Task<Reservation> AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        _dbContext.Reservations.Add(reservation);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return reservation;
    }

    public async Task<Reservation> UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
        return reservation;
    }
}
