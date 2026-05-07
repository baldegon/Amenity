using Reservas.Application.Auth.Dtos;
using Reservas.Application.Common;

namespace Reservas.Application.Auth.Services;

public interface IAuthService
{
    Task<OperationResult<AuthResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);

    Task<OperationResult<AuthResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
