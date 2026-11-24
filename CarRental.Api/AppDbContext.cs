using Microsoft.EntityFrameworkCore;
using CarRental.Api.Models;

namespace CarRental.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Rental> Rentals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>()
            .Property(c => c.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Rental>()
            .Property(r => r.Price)
            .HasPrecision(18, 2);
    }
}