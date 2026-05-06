using Microsoft.EntityFrameworkCore;
using Reservas.Domain.Entities;
using Reservas.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/properties", async (ApplicationDbContext dbContext) =>
{
    var properties = await dbContext.Properties.AsNoTracking().ToListAsync();
    return Results.Ok(properties);
})
.WithName("GetProperties");

app.MapGet("/api/properties/{id:int}", async (int id, ApplicationDbContext dbContext) =>
{
    var property = await dbContext.Properties.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    return property is null ? Results.NotFound() : Results.Ok(property);
})
.WithName("GetPropertyById");

app.MapPost("/api/properties", async (Property property, ApplicationDbContext dbContext) =>
{
    if (string.IsNullOrWhiteSpace(property.Title))
    {
        return Results.BadRequest("Title es obligatorio.");
    }

    if (property.PricePerNight <= 0)
    {
        return Results.BadRequest("PricePerNight debe ser mayor a 0.");
    }

    dbContext.Properties.Add(property);
    await dbContext.SaveChangesAsync();

    return Results.Created($"/api/properties/{property.Id}", property);
})
.WithName("CreateProperty");

app.Run();
