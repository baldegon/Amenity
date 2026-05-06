using Reservas.Domain.Entities;

namespace Reservas.Domain.Tests.Entities;

public class PropertyTests
{
    [Fact]
    public void Property_Allows_Assigning_Core_Data()
    {
        var property = new Property
        {
            Id = 1,
            Title = "Departamento céntrico",
            PricePerNight = 125.50m,
            UserId = 10
        };

        Assert.Equal(1, property.Id);
        Assert.Equal("Departamento céntrico", property.Title);
        Assert.Equal(125.50m, property.PricePerNight);
        Assert.Equal(10, property.UserId);
    }
}
