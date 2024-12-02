using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveOrderBook.Application.Services.WebSocket;
using LiveOrderBook.Domain.Entities;
using LiveOrderBook.Domain.Interfaces.Repository;
using Moq;
using Xunit;

namespace LiveOrderBook.Application.Test.Services.WebSocket;

public class AssetStatisticsServiceTest
{
    private readonly Mock<IAssetPriceRepository> _assetPriceRepositoryMock;
    private readonly AssetStatisticsService _service;

    public AssetStatisticsServiceTest()
    {
        _assetPriceRepositoryMock = new Mock<IAssetPriceRepository>();
        _service = new AssetStatisticsService(_assetPriceRepositoryMock.Object);
    }

    [Fact]
    public async Task GetStatisticsAsync_EmptyTrades_ShouldThrowException()
    {
        _assetPriceRepositoryMock
            .Setup(repo => repo.GetTradesAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<AssetPrice>());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.GetStatisticsAsync("BTC/USD", DateTime.UtcNow));
    }

    [Fact]
    public async Task GetStatisticsAsync_ValidTrades_ShouldReturnCorrectStatistics()
    {
        _assetPriceRepositoryMock
            .Setup(repo => repo.GetTradesAsync("BTC/USD", It.IsAny<DateTime>()))
            .ReturnsAsync(new List<AssetPrice>
            {
                new AssetPrice { Price = 100, Quantity = 2 },
                new AssetPrice { Price = 200, Quantity = 4 },
                new AssetPrice { Price = 150, Quantity = 6 }
            });

        var result = await _service.GetStatisticsAsync("BTC/USD", DateTime.UtcNow);

        Assert.NotNull(result);
        Assert.Equal(200, result.HighestPrice);
        Assert.Equal(100, result.LowestPrice);
        Assert.Equal(150, result.AveragePrice);
        Assert.Equal(4, result.AverageQuantity);
    }

    [Fact]
    public async Task GetAllStatisticsAsync_NoTradesForAssets_ShouldReturnEmptyList()
    {
        _assetPriceRepositoryMock
            .Setup(repo => repo.GetTradesAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<AssetPrice>());

        var result = await _service.GetAllStatisticsAsync(DateTime.UtcNow);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllStatisticsAsync_ValidTradesForMultipleAssets_ShouldReturnCorrectStatistics()
    {
        _assetPriceRepositoryMock
            .Setup(repo => repo.GetTradesAsync("BTC/USD", It.IsAny<DateTime>()))
            .ReturnsAsync(new List<AssetPrice>
            {
                new AssetPrice { Price = 300, Quantity = 10 },
                new AssetPrice { Price = 100, Quantity = 5 }
            });

        _assetPriceRepositoryMock
            .Setup(repo => repo.GetTradesAsync("ETH/USD", It.IsAny<DateTime>()))
            .ReturnsAsync(new List<AssetPrice>
            {
                new AssetPrice { Price = 200, Quantity = 8 },
                new AssetPrice { Price = 50, Quantity = 2 }
            });

        var result = await _service.GetAllStatisticsAsync(DateTime.UtcNow);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        var btcStats = result.FirstOrDefault(r => r.Asset == "BTC/USD");
        Assert.NotNull(btcStats);
        Assert.Equal(300, btcStats.HighestPrice);
        Assert.Equal(100, btcStats.LowestPrice);
        Assert.Equal(200, btcStats.AveragePrice);
        Assert.Equal(7.5m, btcStats.AverageQuantity);

        var ethStats = result.FirstOrDefault(r => r.Asset == "ETH/USD");
        Assert.NotNull(ethStats);
        Assert.Equal(200, ethStats.HighestPrice);
        Assert.Equal(50, ethStats.LowestPrice);
        Assert.Equal(125, ethStats.AveragePrice);
        Assert.Equal(5, ethStats.AverageQuantity);
    }
}