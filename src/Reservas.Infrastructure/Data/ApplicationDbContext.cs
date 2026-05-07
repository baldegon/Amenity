using Microsoft.EntityFrameworkCore;
using Reservas.Domain.Entities;

namespace Reservas.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Property> Properties => Set<Property>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(user =>
        {
            user.HasKey(x => x.Id);
            user.Property(x => x.Email).IsRequired();
            user.Property(x => x.PasswordHash).IsRequired();
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
        });

        base.OnModelCreating(modelBuilder);
    }
}
