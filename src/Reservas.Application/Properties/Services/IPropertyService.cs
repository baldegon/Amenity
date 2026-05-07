using Reservas.Application.Common;
using Reservas.Application.Properties.Dtos;

namespace Reservas.Application.Properties.Services;

public interface IPropertyService
{
    Task<IReadOnlyCollection<PropertyResponseDto>> GetAllAsync(int userId, CancellationToken cancellationToken = default);

    Task<PropertyResponseDto?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default);

    Task<OperationResult<PropertyResponseDto>> CreateAsync(CreatePropertyRequestDto request, int userId, CancellationToken cancellationToken = default);

    Task<OperationResult<PropertyResponseDto>> UpdateAsync(int id, UpdatePropertyRequestDto request, int userId, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);
}
