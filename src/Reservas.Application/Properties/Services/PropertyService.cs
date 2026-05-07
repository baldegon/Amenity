using Reservas.Application.Common;
using Reservas.Application.Properties.Dtos;
using Reservas.Application.Properties.Repositories;
using Reservas.Domain.Entities;

namespace Reservas.Application.Properties.Services;

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;

    public PropertyService(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<IReadOnlyCollection<PropertyResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var properties = await _propertyRepository.GetAllAsync(cancellationToken);

        return properties.Select(MapToResponse).ToArray();
    }

    public async Task<PropertyResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(id, cancellationToken);
        return property is null ? null : MapToResponse(property);
    }

    public async Task<OperationResult<PropertyResponseDto>> CreateAsync(CreatePropertyRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return OperationResult<PropertyResponseDto>.Failure("Title es obligatorio.");
        }

        if (request.PricePerNight <= 0)
        {
            return OperationResult<PropertyResponseDto>.Failure("PricePerNight debe ser mayor a 0.");
        }

        var property = new Property
        {
            Title = request.Title.Trim(),
            PricePerNight = request.PricePerNight,
            UserId = request.UserId
        };

        var createdProperty = await _propertyRepository.AddAsync(property, cancellationToken);

        return OperationResult<PropertyResponseDto>.Success(MapToResponse(createdProperty));
    }

    private static PropertyResponseDto MapToResponse(Property property)
    {
        return new PropertyResponseDto
        {
            Id = property.Id,
            Title = property.Title,
            PricePerNight = property.PricePerNight,
            UserId = property.UserId
        };
    }
}
