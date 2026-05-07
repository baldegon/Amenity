using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reservas.Application.Common;
using Reservas.Application.Properties.Dtos;
using Reservas.Application.Properties.Services;

namespace Reservas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertiesController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PropertyResponseDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var properties = await _propertyService.GetAllAsync(userId.Value, cancellationToken);
        return Ok(properties);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PropertyResponseDto>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var property = await _propertyService.GetByIdAsync(id, userId.Value, cancellationToken);

        if (property is null)
        {
            return NotFound();
        }

        return Ok(property);
    }

    [HttpPost]
    public async Task<ActionResult<PropertyResponseDto>> CreateAsync(CreatePropertyRequestDto request, CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await _propertyService.CreateAsync(request, userId.Value, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PropertyResponseDto>> UpdateAsync(int id, UpdatePropertyRequestDto request, CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await _propertyService.UpdateAsync(id, request, userId.Value, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorType == OperationErrorType.NotFound)
            {
                return NotFound();
            }

            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        var deleted = await _propertyService.DeleteAsync(id, userId.Value, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private int? GetAuthenticatedUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
