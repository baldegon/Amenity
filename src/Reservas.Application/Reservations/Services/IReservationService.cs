using Reservas.Application.Common;
using Reservas.Application.Reservations.Dtos;

namespace Reservas.Application.Reservations.Services;

public interface IReservationService
{
    Task<OperationResult<IReadOnlyCollection<ReservationResponseDto>>> GetByPropertyAsync(int propertyId, int userId, CancellationToken cancellationToken = default);

    Task<OperationResult<ReservationResponseDto>> CreateAsync(int propertyId, CreateReservationRequestDto request, int userId, CancellationToken cancellationToken = default);

    Task<OperationResult<ReservationResponseDto>> CancelAsync(int propertyId, int reservationId, int userId, CancellationToken cancellationToken = default);
}
