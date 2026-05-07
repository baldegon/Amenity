using Reservas.Application.Common;
using Reservas.Application.Properties.Dtos;
using Reservas.Application.Properties.Repositories;
using Reservas.Application.Properties.Services;
using Reservas.Domain.Entities;

namespace Reservas.Api.Tests.Services;

public class PropertyServiceTests
{
    [Fact]
    public async Task GetByIdAsync_WhenPropertyBelongsToAnotherUser_ReturnsNull()
    {
        var repository = new FakePropertyRepository();
        var service = new PropertyService(repository);

        var result = await service.GetByIdAsync(5, 7, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenPropertyBelongsToAnotherUser_ReturnsNotFound()
    {
        var repository = new FakePropertyRepository();
        var service = new PropertyService(repository);

        var result = await service.UpdateAsync(5, new UpdatePropertyRequestDto
        {
            Title = "Depto actualizado",
            PricePerNight = 140
        }, 7, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(OperationErrorType.NotFound, result.ErrorType);
        Assert.False(repository.UpdateCalled);
    }

    [Fact]
    public async Task DeleteAsync_WhenPropertyBelongsToAnotherUser_ReturnsFalse()
    {
        var repository = new FakePropertyRepository();
        var service = new PropertyService(repository);

        var deleted = await service.DeleteAsync(5, 7, CancellationToken.None);

        Assert.False(deleted);
        Assert.False(repository.DeleteCalled);
    }

    private sealed class FakePropertyRepository : IPropertyRepository
    {
        public bool UpdateCalled { get; private set; }

        public bool DeleteCalled { get; private set; }

        public Task<Property> AddAsync(Property property, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(property);
        }

        public Task DeleteAsync(Property property, CancellationToken cancellationToken = default)
        {
            DeleteCalled = true;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyCollection<Property>> GetAllByUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<Property> properties = [];
            return Task.FromResult(properties);
        }

        public Task<Property?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Property?>(null);
        }

        public Task<Property?> GetByIdForUpdateAsync(int id, int userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Property?>(null);
        }

        public Task<Property> UpdateAsync(Property property, CancellationToken cancellationToken = default)
        {
            UpdateCalled = true;
            return Task.FromResult(property);
        }
    }
}
