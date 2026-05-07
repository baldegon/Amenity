using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reservas.Api.Controllers;
using Reservas.Application.Common;
using Reservas.Application.Properties.Dtos;
using Reservas.Application.Properties.Services;

namespace Reservas.Api.Tests.Controllers;

public class PropertiesControllerTests
{
    [Fact]
    public async Task CreateAsync_WithoutAuthenticatedUser_ReturnsUnauthorized()
    {
        var controller = new PropertiesController(new FakePropertyService())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        var result = await controller.CreateAsync(new CreatePropertyRequestDto
        {
            Title = "Depto",
            PricePerNight = 100
        }, CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetAllAsync_WithAuthenticatedUser_ReturnsOk()
    {
        var controller = new PropertiesController(new FakePropertyService())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, "7")],
                    "TestAuth"))
                }
            }
        };

        var result = await controller.GetAllAsync(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsAssignableFrom<IReadOnlyCollection<PropertyResponseDto>>(okResult.Value);
        Assert.Single(payload);
        Assert.Equal(7, payload.Single().UserId);
    }

    [Fact]
    public async Task UpdateAsync_WhenServiceReturnsNotFound_ReturnsNotFound()
    {
        var controller = new PropertiesController(new FakePropertyService())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, "7")],
                    "TestAuth"))
                }
            }
        };

        var result = await controller.UpdateAsync(999, new UpdatePropertyRequestDto
        {
            Title = "Depto actualizado",
            PricePerNight = 150
        }, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenServiceReturnsFalse_ReturnsNotFound()
    {
        var controller = new PropertiesController(new FakePropertyService())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, "7")],
                    "TestAuth"))
                }
            }
        };

        var result = await controller.DeleteAsync(999, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    private sealed class FakePropertyService : IPropertyService
    {
        public Task<OperationResult<PropertyResponseDto>> CreateAsync(CreatePropertyRequestDto request, int userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(OperationResult<PropertyResponseDto>.Success(new PropertyResponseDto
            {
                Id = 1,
                Title = request.Title,
                PricePerNight = request.PricePerNight,
                UserId = userId
            }));
        }

        public Task<IReadOnlyCollection<PropertyResponseDto>> GetAllAsync(int userId, CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<PropertyResponseDto> response =
            [
                new PropertyResponseDto
                {
                    Id = 1,
                    Title = "Depto",
                    PricePerNight = 100,
                    UserId = userId
                }
            ];

            return Task.FromResult(response);
        }

        public Task<PropertyResponseDto?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<PropertyResponseDto?>(new PropertyResponseDto
            {
                Id = id,
                Title = "Depto",
                PricePerNight = 100,
                UserId = userId
            });
        }

        public Task<OperationResult<PropertyResponseDto>> UpdateAsync(int id, UpdatePropertyRequestDto request, int userId, CancellationToken cancellationToken = default)
        {
            if (id == 999)
            {
                return Task.FromResult(OperationResult<PropertyResponseDto>.NotFound("Property no encontrada."));
            }

            return Task.FromResult(OperationResult<PropertyResponseDto>.Success(new PropertyResponseDto
            {
                Id = id,
                Title = request.Title,
                PricePerNight = request.PricePerNight,
                UserId = userId
            }));
        }

        public Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(id != 999);
        }
    }
}
