namespace Reservas.Domain.Entities;

public class Reservation
{
    public int Id { get; set; }

    public int PropertyId { get; set; }

    public Property? Property { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal TotalPrice { get; set; }

    public ReservationStatus Status { get; private set; } = ReservationStatus.Active;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? CancelledAtUtc { get; private set; }

    public bool HasValidDateRange() => StartDate < EndDate;

    public int GetNightCount() => EndDate.DayNumber - StartDate.DayNumber;

    public decimal CalculateTotalPrice(decimal pricePerNight) => GetNightCount() * pricePerNight;

    public bool Overlaps(DateOnly startDate, DateOnly endDate)
    {
        return Status == ReservationStatus.Active
            && StartDate < endDate
            && startDate < EndDate;
    }

    public void Cancel(DateTime cancelledAtUtc)
    {
        Status = ReservationStatus.Canceled;
        CancelledAtUtc = cancelledAtUtc;
    }
}

public enum ReservationStatus
{
    Active = 1,
    Canceled = 2
}
