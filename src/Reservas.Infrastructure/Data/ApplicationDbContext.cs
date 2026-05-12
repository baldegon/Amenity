using Microsoft.EntityFrameworkCore;
using Reservas.Domain.Entities;

namespace Reservas.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Property> Properties => Set<Property>();

    public DbSet<Reservation> Reservations => Set<Reservation>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(user =>
        {
            user.HasKey(x => x.Id);
            user.Property(x => x.Email).IsRequired();
            user.Property(x => x.PasswordHash).IsRequired();
            user.Property(x => x.Role).IsRequired();
            user.Property(x => x.CreatedAtUtc).IsRequired();
            user.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Property>(property =>
        {
            property.Property(p => p.Title)
                .IsRequired();

            property.Property(p => p.PricePerNight)
                .HasColumnType("decimal(18,2)");

            property.HasOne(p => p.User)
                .WithMany(user => user.Properties)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            property.HasMany(p => p.Reservations)
                .WithOne(reservation => reservation.Property)
                .HasForeignKey(reservation => reservation.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Reservation>(reservation =>
        {
            reservation.HasKey(x => x.Id);

            reservation.Property(x => x.StartDate)
                .IsRequired();

            reservation.Property(x => x.EndDate)
                .IsRequired();

            reservation.Property(x => x.TotalPrice)
                .HasColumnType("decimal(18,2)");

            reservation.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

            reservation.Property(x => x.CreatedAtUtc)
                .IsRequired();

            reservation.HasIndex(x => new { x.PropertyId, x.StartDate, x.EndDate });
        });

        base.OnModelCreating(modelBuilder);
    }
}
