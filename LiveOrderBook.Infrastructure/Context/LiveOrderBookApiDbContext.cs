using LiveOrderBook.Domain.Entities;
using LiveOrderBook.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace LiveOrderBook.Infrastructure.Context;

public class LiveOrderBookApiDbContext : DbContext
{
    public LiveOrderBookApiDbContext(DbContextOptions<LiveOrderBookApiDbContext> options)
        : base(options)
    {
    }

    public DbSet<AssetPrice> AssetPrices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AssetPriceMap());
    }
}