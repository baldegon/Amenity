using Reservas.Application.Common;
using Reservas.Application.Properties.Repositories;
using Reservas.Application.Reservations.Dtos;
using Reservas.Application.Reservations.Repositories;
using Reservas.Domain.Entities;

namespace Reservas.Application.Reservations.Services;

public class ReservationService : IReservationService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IReservationRepository _reservationRepository;

    public ReservationService(IPropertyRepository propertyRepository, IReservationRepository reservationRepository)
    {
        _propertyRepository = propertyRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<OperationResult<IReadOnlyCollection<ReservationResponseDto>>> GetByPropertyAsync(int propertyId, int userId, CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId, userId, cancellationToken);

        if (property is null)
        {
            return OperationResult<IReadOnlyCollection<ReservationResponseDto>>.NotFound("Property no encontrada.");
        }

        var reservations = await _reservationRepository.GetByPropertyAsync(propertyId, cancellationToken);

        return OperationResult<IReadOnlyCollection<ReservationResponseDto>>.Success(reservations.Select(MapToResponse).ToArray());
    }

    public async Task<OperationResult<ReservationResponseDto>> CreateAsync(int propertyId, CreateReservationRequestDto request, int userId, CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId, userId, cancellationToken);

        if (property is null)
        {
            return OperationResult<ReservationResponseDto>.NotFound("Property no encontrada.");
        }

        var reservation = new Reservation
        {
            PropertyId = propertyId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            CreatedAtUtc = DateTime.UtcNow
        };

        if (!reservation.HasValidDateRange())
        {
            return OperationResult<ReservationResponseDto>.Failure("StartDate debe ser menor a EndDate.");
        }

        var hasOverlap = await _reservationRepository.HasOverlappingReservationAsync(propertyId, request.StartDate, request.EndDate, cancellationToken);

        if (hasOverlap)
        {
            return OperationResult<ReservationResponseDto>.Failure("Ya existe una reserva solapada para esa property.");
        }

        reservation.TotalPrice = reservation.CalculateTotalPrice(property.PricePerNight);

        var createdReservation = await _reservationRepository.AddAsync(reservation, cancellationToken);

        return OperationResult<ReservationResponseDto>.Success(MapToResponse(createdReservation));
    }

    public async Task<OperationResult<ReservationResponseDto>> CancelAsync(int propertyId, int reservationId, int userId, CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId, userId, cancellationToken);

        if (property is null)
        {
            return OperationResult<ReservationResponseDto>.NotFound("Property no encontrada.");
        }

        var reservation = await _reservationRepository.GetByIdAsync(reservationId, propertyId, cancellationToken);

        if (reservation is null)
        {
            return OperationResult<ReservationResponseDto>.NotFound("Reserva no encontrada.");
        }

        reservation.Cancel(DateTime.UtcNow);

        var updatedReservation = await _reservationRepository.UpdateAsync(reservation, cancellationToken);

        return OperationResult<ReservationResponseDto>.Success(MapToResponse(updatedReservation));
    }

    private static ReservationResponseDto MapToResponse(Reservation reservation)
    {
        return new ReservationResponseDto
        {
            Id = reservation.Id,
            PropertyId = reservation.PropertyId,
            StartDate = reservation.StartDate,
            EndDate = reservation.EndDate,
            TotalPrice = reservation.TotalPrice,
            Status = reservation.Status.ToString(),
            CreatedAtUtc = reservation.CreatedAtUtc,
            CancelledAtUtc = reservation.CancelledAtUtc
        };
    }
}
