using Microsoft.AspNetCore.Mvc;
using Reservas.Application.Properties.Dtos;
using Reservas.Application.Properties.Services;

namespace Reservas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var properties = await _propertyService.GetAllAsync(cancellationToken);
        return Ok(properties);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PropertyResponseDto>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var property = await _propertyService.GetByIdAsync(id, cancellationToken);

        if (property is null)
        {
            return NotFound();
        }

        return Ok(property);
    }

    [HttpPost]
    public async Task<ActionResult<PropertyResponseDto>> CreateAsync(CreatePropertyRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _propertyService.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Value!.Id }, result.Value);
    }
}
