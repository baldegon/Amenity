namespace Reservas.Application.Properties.Dtos;

public class PropertyResponseDto
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public decimal PricePerNight { get; set; }

    public int UserId { get; set; }
}
