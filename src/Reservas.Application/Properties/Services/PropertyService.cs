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

    public async Task<IReadOnlyCollection<PropertyResponseDto>> GetAllAsync(int userId, CancellationToken cancellationToken = default)
    {
        var properties = await _propertyRepository.GetAllByUserAsync(userId, cancellationToken);

        return properties.Select(MapToResponse).ToArray();
    }

    public async Task<PropertyResponseDto?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(id, userId, cancellationToken);
        return property is null ? null : MapToResponse(property);
    }

    public async Task<OperationResult<PropertyResponseDto>> CreateAsync(CreatePropertyRequestDto request, int userId, CancellationToken cancellationToken = default)
    {
        var validationError = ValidatePropertyRequest(request.Title, request.PricePerNight);

        if (validationError is not null)
        {
            return OperationResult<PropertyResponseDto>.Failure(validationError);
        }

        var normalizedTitle = request.Title!.Trim();

        var property = new Property
        {
            Title = normalizedTitle,
            PricePerNight = request.PricePerNight,
            UserId = userId
        };

        var createdProperty = await _propertyRepository.AddAsync(property, cancellationToken);

        return OperationResult<PropertyResponseDto>.Success(MapToResponse(createdProperty));
    }

    public async Task<OperationResult<PropertyResponseDto>> UpdateAsync(int id, UpdatePropertyRequestDto request, int userId, CancellationToken cancellationToken = default)
    {
        var validationError = ValidatePropertyRequest(request.Title, request.PricePerNight);

        if (validationError is not null)
        {
            return OperationResult<PropertyResponseDto>.Failure(validationError);
        }

        var normalizedTitle = request.Title!.Trim();

        var property = await _propertyRepository.GetByIdForUpdateAsync(id, userId, cancellationToken);

        if (property is null)
        {
            return OperationResult<PropertyResponseDto>.NotFound("Property no encontrada.");
        }

        property.Title = normalizedTitle;
        property.PricePerNight = request.PricePerNight;

        var updatedProperty = await _propertyRepository.UpdateAsync(property, cancellationToken);

        return OperationResult<PropertyResponseDto>.Success(MapToResponse(updatedProperty));
    }

    public async Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdForUpdateAsync(id, userId, cancellationToken);

        if (property is null)
        {
            return false;
        }

        await _propertyRepository.DeleteAsync(property, cancellationToken);
        return true;
    }

    private static string? ValidatePropertyRequest(string? title, decimal pricePerNight)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return "Title es obligatorio.";
        }

        if (pricePerNight <= 0)
        {
            return "PricePerNight debe ser mayor a 0.";
        }

        return null;
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
