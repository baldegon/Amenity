namespace Reservas.Application.Auth.Dtos;

public class AuthResponseDto
{
    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string AccessToken { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }
}
