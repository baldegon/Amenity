namespace Reservas.Application.Reservations.Dtos;

public class ReservationResponseDto
{
    public int Id { get; set; }

    public int PropertyId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? CancelledAtUtc { get; set; }
}
