using Reservas.Domain.Entities;

namespace Reservas.Domain.Tests.Entities;

public class ReservationTests
{
    [Fact]
    public void HasValidDateRange_WhenStartDateIsBeforeEndDate_ReturnsTrue()
    {
        var reservation = new Reservation
        {
            StartDate = new DateOnly(2026, 5, 10),
            EndDate = new DateOnly(2026, 5, 12)
        };

        Assert.True(reservation.HasValidDateRange());
    }

    [Fact]
    public void CalculateTotalPrice_UsesNightCountAndPricePerNight()
    {
        var reservation = new Reservation
        {
            StartDate = new DateOnly(2026, 5, 10),
            EndDate = new DateOnly(2026, 5, 13)
        };

        var totalPrice = reservation.CalculateTotalPrice(150m);

        Assert.Equal(450m, totalPrice);
    }

    [Fact]
    public void Overlaps_WhenRangesIntersectAndReservationIsActive_ReturnsTrue()
    {
        var reservation = new Reservation
        {
            StartDate = new DateOnly(2026, 5, 10),
            EndDate = new DateOnly(2026, 5, 15)
        };

        var overlaps = reservation.Overlaps(new DateOnly(2026, 5, 12), new DateOnly(2026, 5, 18));

        Assert.True(overlaps);
    }

    [Fact]
    public void Overlaps_WhenReservationWasCanceled_ReturnsFalse()
    {
        var reservation = new Reservation
        {
            StartDate = new DateOnly(2026, 5, 10),
            EndDate = new DateOnly(2026, 5, 15)
        };

        reservation.Cancel(DateTime.UtcNow);

        var overlaps = reservation.Overlaps(new DateOnly(2026, 5, 12), new DateOnly(2026, 5, 18));

        Assert.False(overlaps);
    }
}
