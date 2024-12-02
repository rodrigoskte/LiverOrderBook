using LiveOrderBook.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LiveOrderBook.Infrastructure.Test.Migrations;

public class LiveOrderBookApiDbContextTest
{
    [Fact]
    public void Should_Apply_Migrations_And_Create_Database()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<LiveOrderBookApiDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new LiveOrderBookApiDbContext(options);

        // Act
        context.Database.EnsureCreated();

        // Assert
        Assert.True(context.Database.CanConnect());
        Assert.NotNull(context.Model.FindEntityType(typeof(LiveOrderBook.Domain.Entities.AssetPrice)));
    }
}