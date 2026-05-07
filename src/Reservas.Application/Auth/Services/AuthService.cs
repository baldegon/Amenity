using System.Globalization;
using Reservas.Application.Auth.Dtos;
using Reservas.Application.Auth.Repositories;
using Reservas.Application.Common;
using Reservas.Domain.Entities;

namespace Reservas.Application.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<OperationResult<AuthResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        var passwordError = ValidatePassword(request.Password);

        if (email is null)
        {
            return OperationResult<AuthResponseDto>.Failure("Email inválido.");
        }

        if (passwordError is not null)
        {
            return OperationResult<AuthResponseDto>.Failure(passwordError);
        }

        var exists = await _userRepository.ExistsByEmailAsync(email, cancellationToken);

        if (exists)
        {
            return OperationResult<AuthResponseDto>.Failure("Ya existe un usuario con ese email.");
        }

        var user = new User
        {
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password!),
            CreatedAtUtc = DateTime.UtcNow
        };

        var createdUser = await _userRepository.AddAsync(user, cancellationToken);
        var token = _tokenGenerator.Generate(createdUser);

        return OperationResult<AuthResponseDto>.Success(MapToResponse(createdUser, token));
    }

    public async Task<OperationResult<AuthResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);

        if (email is null || string.IsNullOrWhiteSpace(request.Password))
        {
            return OperationResult<AuthResponseDto>.Failure("Credenciales inválidas.");
        }

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return OperationResult<AuthResponseDto>.Failure("Credenciales inválidas.");
        }

        var token = _tokenGenerator.Generate(user);

        return OperationResult<AuthResponseDto>.Success(MapToResponse(user, token));
    }

    private static AuthResponseDto MapToResponse(User user, (string AccessToken, DateTime ExpiresAtUtc) token)
    {
        return new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = token.AccessToken,
            ExpiresAtUtc = token.ExpiresAtUtc
        };
    }

    private static string? NormalizeEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var normalizedEmail = email.Trim().ToLower(CultureInfo.InvariantCulture);
        return normalizedEmail.Contains('@') ? normalizedEmail : null;
    }

    private static string? ValidatePassword(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return "Password es obligatorio.";
        }

        if (password.Length < 8)
        {
            return "Password debe tener al menos 8 caracteres.";
        }

        return null;
    }
}
