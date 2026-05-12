using Reservas.Application.Common;
using Reservas.Application.Properties.Repositories;
using Reservas.Application.Reservations.Dtos;
using Reservas.Application.Reservations.Repositories;
using Reservas.Application.Reservations.Services;
using Reservas.Domain.Entities;

namespace Reservas.Api.Tests.Services;

public class ReservationServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenPropertyBelongsToAnotherUser_ReturnsNotFound()
    {
        var propertyRepository = new FakePropertyRepository();
        var reservationRepository = new FakeReservationRepository();
        var service = new ReservationService(propertyRepository, reservationRepository);

        var result = await service.CreateAsync(20, new CreateReservationRequestDto
        {
            StartDate = new DateOnly(2026, 5, 10),
            EndDate = new DateOnly(2026, 5, 12)
        }, 99, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(OperationErrorType.NotFound, result.ErrorType);
        Assert.False(reservationRepository.AddCalled);
    }

    [Fact]
    public async Task CreateAsync_WhenDateRangeIsInvalid_ReturnsValidationError()
    {
        var propertyRepository = new FakePropertyRepository();
        var reservationRepository = new FakeReservationRepository();
        var service = new ReservationService(propertyRepository, reservationRepository);

        var result = await service.CreateAsync(10, new CreateReservationRequestDto
        {
            StartDate = new DateOnly(2026, 5, 12),
            EndDate = new DateOnly(2026, 5, 12)
        }, 7, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(OperationErrorType.Validation, result.ErrorType);
        Assert.False(reservationRepository.AddCalled);
    }

    [Fact]
    public async Task CreateAsync_WhenReservationOverlaps_ReturnsValidationError()
    {
        var propertyRepository = new FakePropertyRepository();
        var reservationRepository = new FakeReservationRepository { HasOverlap = true };
        var service = new ReservationService(propertyRepository, reservationRepository);

        var result = await service.CreateAsync(10, new CreateReservationRequestDto
        {
            StartDate = new DateOnly(2026, 5, 11),
            EndDate = new DateOnly(2026, 5, 14)
        }, 7, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(OperationErrorType.Validation, result.ErrorType);
        Assert.False(reservationRepository.AddCalled);
    }

    [Fact]
    public async Task CreateAsync_WhenRequestIsValid_CalculatesTotalPrice()
    {
        var propertyRepository = new FakePropertyRepository();
        var reservationRepository = new FakeReservationRepository();
        var service = new ReservationService(propertyRepository, reservationRepository);

        var result = await service.CreateAsync(10, new CreateReservationRequestDto
        {
            StartDate = new DateOnly(2026, 5, 10),
            EndDate = new DateOnly(2026, 5, 13)
        }, 7, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(360m, result.Value!.TotalPrice);
        Assert.True(reservationRepository.AddCalled);
    }

    [Fact]
    public async Task GetByPropertyAsync_WhenPropertyBelongsToAnotherUser_ReturnsNotFound()
    {
        var propertyRepository = new FakePropertyRepository();
        var reservationRepository = new FakeReservationRepository();
        var service = new ReservationService(propertyRepository, reservationRepository);

        var result = await service.GetByPropertyAsync(20, 99, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(OperationErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task CancelAsync_WhenPropertyBelongsToAnotherUser_ReturnsNotFound()
    {
        var propertyRepository = new FakePropertyRepository();
        var reservationRepository = new FakeReservationRepository();
        var service = new ReservationService(propertyRepository, reservationRepository);

        var result = await service.CancelAsync(20, 1, 99, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(OperationErrorType.NotFound, result.ErrorType);
        Assert.False(reservationRepository.UpdateCalled);
    }

    [Fact]
    public async Task CancelAsync_WhenReservationExists_CancelsIt()
    {
        var propertyRepository = new FakePropertyRepository();
        var reservationRepository = new FakeReservationRepository();
        var service = new ReservationService(propertyRepository, reservationRepository);

        var result = await service.CancelAsync(10, 1, 7, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Canceled", result.Value!.Status);
        Assert.NotNull(result.Value.CancelledAtUtc);
        Assert.True(reservationRepository.UpdateCalled);
    }

    private sealed class FakePropertyRepository : IPropertyRepository
    {
        public Task<Property> AddAsync(Property property, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(property);
        }

        public Task DeleteAsync(Property property, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<Property>> GetAllByUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<Property> properties = [];
            return Task.FromResult(properties);
        }

        public Task<Property?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
        {
            if (id == 10 && userId == 7)
            {
                return Task.FromResult<Property?>(new Property
                {
                    Id = 10,
                    Title = "Casa",
                    PricePerNight = 120m,
                    UserId = 7
                });
            }

            return Task.FromResult<Property?>(null);
        }

        public Task<Property?> GetByIdForUpdateAsync(int id, int userId, CancellationToken cancellationToken = default)
        {
            return GetByIdAsync(id, userId, cancellationToken);
        }

        public Task<Property> UpdateAsync(Property property, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(property);
        }
    }

    private sealed class FakeReservationRepository : IReservationRepository
    {
        public bool HasOverlap { get; set; }

        public bool AddCalled { get; private set; }

        public bool UpdateCalled { get; private set; }

        private readonly List<Reservation> _reservations =
        [
            new Reservation
            {
                Id = 1,
                PropertyId = 10,
                StartDate = new DateOnly(2026, 5, 10),
                EndDate = new DateOnly(2026, 5, 12),
                TotalPrice = 240m,
                CreatedAtUtc = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        ];

        public Task<Reservation> AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
        {
            AddCalled = true;
            reservation.Id = 2;
            _reservations.Add(reservation);
            return Task.FromResult(reservation);
        }

        public Task<IReadOnlyCollection<Reservation>> GetByPropertyAsync(int propertyId, CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<Reservation> reservations = _reservations.Where(x => x.PropertyId == propertyId).ToArray();
            return Task.FromResult(reservations);
        }

        public Task<Reservation?> GetByIdAsync(int reservationId, int propertyId, CancellationToken cancellationToken = default)
        {
            var reservation = _reservations.FirstOrDefault(x => x.Id == reservationId && x.PropertyId == propertyId);
            return Task.FromResult(reservation);
        }

        public Task<bool> HasOverlappingReservationAsync(int propertyId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HasOverlap);
        }

        public Task<Reservation> UpdateAsync(Reservation reservation, CancellationToken cancellationToken = default)
        {
            UpdateCalled = true;
            return Task.FromResult(reservation);
        }
    }
}
