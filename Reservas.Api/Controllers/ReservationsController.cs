using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reservas.Application.Common;
using Reservas.Application.Reservations.Dtos;
using Reservas.Application.Reservations.Services;

namespace Reservas.Api.Controllers;

[ApiController]
[Route("api/properties/{propertyId:int}/reservations")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ReservationResponseDto>>> GetByPropertyAsync(int propertyId, CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await _reservationService.GetByPropertyAsync(propertyId, userId.Value, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<ReservationResponseDto>> CreateAsync(int propertyId, CreateReservationRequestDto request, CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await _reservationService.CreateAsync(propertyId, request, userId.Value, cancellationToken);

        if (!result.IsSuccess)
        {
            return ToErrorResult<ReservationResponseDto>(result);
        }

        return CreatedAtAction(nameof(GetByPropertyAsync), new { propertyId }, result.Value);
    }

    [HttpPatch("{reservationId:int}/cancel")]
    public async Task<ActionResult<ReservationResponseDto>> CancelAsync(int propertyId, int reservationId, CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await _reservationService.CancelAsync(propertyId, reservationId, userId.Value, cancellationToken);
        return ToActionResult(result);
    }

    private ActionResult<T> ToActionResult<T>(OperationResult<T> result)
    {
        if (!result.IsSuccess)
        {
            return ToErrorResult<T>(result);
        }

        return Ok(result.Value);
    }

    private ActionResult<T> ToErrorResult<T>(OperationResult<T> result)
    {
        if (result.ErrorType == OperationErrorType.NotFound)
        {
            return NotFound();
        }

        return BadRequest(result.ErrorMessage);
    }

    private int? GetAuthenticatedUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
