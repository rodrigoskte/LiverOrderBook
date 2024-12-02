using System;
using System.Threading.Tasks;
using LiveOrderBook.Application.Dtos.WebSocket;
using LiveOrderBook.Application.Services.WebSocket;
using LiveOrderBook.Application.Validators;
using LiveOrderBook.Domain.Entities;
using LiveOrderBook.Domain.Interfaces.Repository;
using Moq;
using Xunit;

namespace LiveOrderBook.Application.Test.Services.WebSocket;

public class TradeProcessorServiceTest
{
    private readonly Mock<IAssetPriceRepository> _assetPriceRepositoryMock;
    private readonly TradeProcessorService _service;
    private readonly AssetPriceValidator _validator;

    public TradeProcessorServiceTest()
    {
        _assetPriceRepositoryMock = new Mock<IAssetPriceRepository>();
        _validator = new AssetPriceValidator();
        _service = new TradeProcessorService(_assetPriceRepositoryMock.Object, _validator);
    }
    
    [Fact]
    public async Task ProcessTradeAsync_InvalidTradeDto_ShouldThrowArgumentException()
    {
        var tradeDto = new TradeMessageDto
        {
            Asset = null,
            Price = -10,  
            Quantity = 0, 
            Timestamp = DateTime.UtcNow
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.ProcessTradeAsync(tradeDto));
    }

    [Fact]
    public async Task ProcessTradeAsync_ValidTrade_ShouldCallSaveAsyncOnce()
    {
        var tradeDto = new TradeMessageDto
        {
            Asset = "BTC/USD",
            Price = 30000,
            Quantity = 0.5m,
            Timestamp = DateTime.UtcNow
        };

        await _service.ProcessTradeAsync(tradeDto);

        _assetPriceRepositoryMock.Verify(
            repo => repo.SaveAsync(It.Is<AssetPrice>(ap =>
                ap.Asset == tradeDto.Asset &&
                ap.Price == tradeDto.Price &&
                ap.Quantity == tradeDto.Quantity &&
                ap.Timestamp == tradeDto.Timestamp
            )), Times.Once);
    }

    [Fact]
    public async Task ProcessTradeAsync_ValidTrade_ShouldNotThrowException()
    {
        var tradeDto = new TradeMessageDto
        {
            Asset = "ETH/USD",
            Price = 2000,
            Quantity = 1.2m,
            Timestamp = DateTime.UtcNow
        };

        var exception = await Record.ExceptionAsync(() => _service.ProcessTradeAsync(tradeDto));
        Assert.Null(exception);
    }
}