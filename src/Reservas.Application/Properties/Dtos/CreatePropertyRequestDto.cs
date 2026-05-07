namespace Reservas.Application.Properties.Dtos;

public class CreatePropertyRequestDto
{
    public string? Title { get; set; }

    public decimal PricePerNight { get; set; }

    public int UserId { get; set; }
}
