using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reservas.Api.Controllers;
using Reservas.Application.Common;
using Reservas.Application.Reservations.Dtos;
using Reservas.Application.Reservations.Services;

namespace Reservas.Api.Tests.Controllers;

public class ReservationsControllerTests
{
    [Fact]
    public async Task GetByPropertyAsync_WithoutAuthenticatedUser_ReturnsUnauthorized()
    {
        var controller = new ReservationsController(new FakeReservationService())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = await controller.GetByPropertyAsync(10, CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task CreateAsync_WhenServiceReturnsNotFound_ReturnsNotFound()
    {
        var controller = CreateController();

        var result = await controller.CreateAsync(999, new CreateReservationRequestDto
        {
            StartDate = new DateOnly(2026, 5, 10),
            EndDate = new DateOnly(2026, 5, 12)
        }, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CancelAsync_WhenServiceReturnsReservation_ReturnsOk()
    {
        var controller = CreateController();

        var result = await controller.CancelAsync(10, 1, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<ReservationResponseDto>(okResult.Value);
        Assert.Equal("Canceled", payload.Status);
    }

    private static ReservationsController CreateController()
    {
        return new ReservationsController(new FakeReservationService())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, "7")],
                    "TestAuth"))
                }
            }
        };
    }

    private sealed class FakeReservationService : IReservationService
    {
        public Task<OperationResult<ReservationResponseDto>> CancelAsync(int propertyId, int reservationId, int userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(OperationResult<ReservationResponseDto>.Success(new ReservationResponseDto
            {
                Id = reservationId,
                PropertyId = propertyId,
                StartDate = new DateOnly(2026, 5, 10),
                EndDate = new DateOnly(2026, 5, 12),
                TotalPrice = 200m,
                Status = "Canceled",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-2),
                CancelledAtUtc = DateTime.UtcNow
            }));
        }

        public Task<OperationResult<ReservationResponseDto>> CreateAsync(int propertyId, CreateReservationRequestDto request, int userId, CancellationToken cancellationToken = default)
        {
            if (propertyId == 999)
            {
                return Task.FromResult(OperationResult<ReservationResponseDto>.NotFound("Property no encontrada."));
            }

            return Task.FromResult(OperationResult<ReservationResponseDto>.Success(new ReservationResponseDto
            {
                Id = 1,
                PropertyId = propertyId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalPrice = 200m,
                Status = "Active",
                CreatedAtUtc = DateTime.UtcNow
            }));
        }

        public Task<OperationResult<IReadOnlyCollection<ReservationResponseDto>>> GetByPropertyAsync(int propertyId, int userId, CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<ReservationResponseDto> reservations = [];
            return Task.FromResult(OperationResult<IReadOnlyCollection<ReservationResponseDto>>.Success(reservations));
        }
    }
}
