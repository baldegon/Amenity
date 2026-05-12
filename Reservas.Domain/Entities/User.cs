namespace Reservas.Domain.Entities;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Owner";

    public DateTime CreatedAtUtc { get; set; }

    public ICollection<Property> Properties { get; set; } = new List<Property>();
}
