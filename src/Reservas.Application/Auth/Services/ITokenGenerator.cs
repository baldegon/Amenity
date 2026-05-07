using Reservas.Domain.Entities;

namespace Reservas.Application.Auth.Services;

public interface ITokenGenerator
{
    (string AccessToken, DateTime ExpiresAtUtc) Generate(User user);
}
