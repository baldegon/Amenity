namespace Reservas.Application.Reservations.Dtos;

public class CreateReservationRequestDto
{
    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }
}
