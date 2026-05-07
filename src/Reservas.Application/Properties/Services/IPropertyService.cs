using Reservas.Application.Common;
using Reservas.Application.Properties.Dtos;

namespace Reservas.Application.Properties.Services;

public interface IPropertyService
{
    Task<IReadOnlyCollection<PropertyResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PropertyResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<OperationResult<PropertyResponseDto>> CreateAsync(CreatePropertyRequestDto request, CancellationToken cancellationToken = default);
}
