using Reservas.Domain.Entities;

namespace Reservas.Application.Reservations.Repositories;

public interface IReservationRepository
{
    Task<IReadOnlyCollection<Reservation>> GetByPropertyAsync(int propertyId, CancellationToken cancellationToken = default);

    Task<Reservation?> GetByIdAsync(int reservationId, int propertyId, CancellationToken cancellationToken = default);

    Task<bool> HasOverlappingReservationAsync(int propertyId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

    Task<Reservation> AddAsync(Reservation reservation, CancellationToken cancellationToken = default);

    Task<Reservation> UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default);
}
