using LiveOrderBook.Domain.Entities;
using LiveOrderBook.Infrastructure.Context;
using LiveOrderBook.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LiveOrderBook.Infrastructure.Test.Repository;

public class AssetPriceRepositoryTest
{
    private readonly DbContextOptions<LiveOrderBookApiDbContext> _dbOptions;

    public AssetPriceRepositoryTest()
    {
        // Banco de dados em memória para testes
        _dbOptions = new DbContextOptionsBuilder<LiveOrderBookApiDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task SaveAsync_Should_Add_AssetPrice_To_Database()
    {
        // Arrange
        var context = new LiveOrderBookApiDbContext(_dbOptions);
        var repository = new AssetPriceRepository(context);

        var trade = new AssetPrice
        {
            Asset = "BTC/USD",
            Price = 50000.00m,
            Quantity = 0.5m,
            Timestamp = DateTime.UtcNow
        };

        // Act
        await repository.SaveAsync(trade);

        // Assert
        Assert.Equal(1, context.AssetPrices.Count());
        Assert.Equal("BTC/USD", context.AssetPrices.First().Asset);
    }

    [Fact]
    public async Task GetTradesAsync_Should_Return_Trades_Since_Specified_Date()
    {
        // Arrange
        var context = new LiveOrderBookApiDbContext(_dbOptions);
        var repository = new AssetPriceRepository(context);

        var trade1 = new AssetPrice
        {
            Asset = "BTC/USD",
            Price = 60000.00m,
            Quantity = 0.2m,
            Timestamp = DateTime.UtcNow.AddMinutes(-10)
        };

        var trade2 = new AssetPrice
        {
            Asset = "BTC/USD",
            Price = 61000.00m,
            Quantity = 0.3m,
            Timestamp = DateTime.UtcNow.AddMinutes(-5)
        };

        await repository.SaveAsync(trade1);
        await repository.SaveAsync(trade2);

        // Act
        var result = await repository.GetTradesAsync("BTC/USD", DateTime.UtcNow.AddMinutes(-7));

        // Assert
        Assert.Single(result);
        Assert.Equal(61000.00m, result.First().Price);
    }
}