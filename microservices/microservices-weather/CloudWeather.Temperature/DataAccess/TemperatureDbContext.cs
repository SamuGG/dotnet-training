using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Temperature.DataAccess;

public class TemperatureDbContext : DbContext
{
    public TemperatureDbContext() {}
    public TemperatureDbContext(DbContextOptions options) : base(options) {}

    public DbSet<Temperature> Temperature { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        SnakeCaseIdentityTableNames(modelBuilder);
    }

    private static void SnakeCaseIdentityTableNames(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Temperature>(builder => builder.ToTable("temperature"));
    }
}