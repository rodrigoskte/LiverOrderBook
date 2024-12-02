using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveOrderBook.Application.Dtos;
using LiveOrderBook.Application.Services.PriceSimulator;
using LiveOrderBook.Domain.Entities;
using LiveOrderBook.Domain.Interfaces.Repository;
using Moq;
using Xunit;

namespace LiveOrderBook.Application.Test.Services.PriceSimulator;

public class PriceSimulationServiceTest
{
    private readonly Mock<IAssetPriceRepository> _assetPriceRepositoryMock;
    private readonly PriceSimulationService _service;
    
    public PriceSimulationServiceTest()
    {
        _assetPriceRepositoryMock = new Mock<IAssetPriceRepository>();
        _service = new PriceSimulationService(_assetPriceRepositoryMock.Object);
    }

    [Fact]
    public async Task SimulatePriceAsync_Request_Null_ShouldThrowArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _service.SimulatePriceAsync(null));
    }
    
    [Fact]
    public async Task SimulatePriceAsync_InvalidOperation_Deve_Retornar_ThrowArgumentException()
    {
        var request = new SimulationRequestDto
        {
            Operation = "",
            Quantity = 10,
            Asset = "BTC/USD"
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.SimulatePriceAsync(request));
    }
    
    [Fact]
    public async Task SimulatePriceAsync_Quantidade_Zero_Negativa_Deve_Retornar_ThrowArgumentException()
    {
        var request = new SimulationRequestDto
        {
            Operation = "compra",
            Quantity = 0,
            Asset = "BTC/USD"
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.SimulatePriceAsync(request));
    }
    
    [Fact]
    public async Task SimulatePriceAsync_Nao_Encontrado_Trades_Deve_Retornar_ThrowKeyNotFoundException()
    {
        _assetPriceRepositoryMock
            .Setup(repo => repo.GetTradesAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<AssetPrice>());

        var request = new SimulationRequestDto
        {
            Operation = "compra",
            Quantity = 10,
            Asset = "BTC/USD"
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.SimulatePriceAsync(request));
    }
    
    [Fact]
    public async Task SimulatePriceAsync_PurchaseQuantityNotFulfilled_Deve_Retornar_ThrowInvalidOperationException()
    {
        _assetPriceRepositoryMock
            .Setup(repo => repo.GetTradesAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<AssetPrice>
            {
                new AssetPrice { Price = 100, Quantity = 5 }
            });

        var request = new SimulationRequestDto
        {
            Operation = "compra",
            Quantity = 10,
            Asset = "BTC/USD"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.SimulatePriceAsync(request));
    }
    
    [Fact]
    public async Task SimulatePriceAsync_SuccessfulPurchase_Deve_Retornar_ReturnCorrectResponse()
    {
        _assetPriceRepositoryMock
            .Setup(repo => repo.GetTradesAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<AssetPrice>
            {
                new AssetPrice { Price = 100, Quantity = 5 },
                new AssetPrice { Price = 200, Quantity = 10 }
            });

        var request = new SimulationRequestDto
        {
            Operation = "compra",
            Quantity = 10,
            Asset = "BTC/USD"
        };

        var response = await _service.SimulatePriceAsync(request);

        Assert.NotNull(response);
        Assert.Equal(10, response.RequestedQuantity);
        Assert.Equal("BTC/USD", response.Asset);
        Assert.Equal("compra", response.Operation);
        Assert.Equal(1500, response.TotalPrice);
        Assert.Equal(2, response.UsedTrades.Count);
    }
    
    [Fact]
    public async Task SimulatePriceAsync_SuccessfulSale_ShouldReturnCorrectResponse()
    {
        _assetPriceRepositoryMock
            .Setup(repo => repo.GetTradesAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<AssetPrice>
            {
                new AssetPrice { Price = 200, Quantity = 10 },
                new AssetPrice { Price = 100, Quantity = 5 }
            });

        var request = new SimulationRequestDto
        {
            Operation = "venda",
            Quantity = 10,
            Asset = "BTC/USD"
        };

        var response = await _service.SimulatePriceAsync(request);

        Assert.NotNull(response);
        Assert.Equal(10, response.RequestedQuantity);
        Assert.Equal("BTC/USD", response.Asset);
        Assert.Equal("venda", response.Operation);
        Assert.Equal(2000, response.TotalPrice);
        Assert.Equal(1, response.UsedTrades.Count);
    }
    
    [Fact]
    public async Task SimulatePriceAsync_Nao_Venda_Nao_Compra_DeveRetornarArgumentException()
    {
        _assetPriceRepositoryMock
            .Setup(repo => repo.GetTradesAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<AssetPrice>
            {
                new AssetPrice { Price = 200, Quantity = 10 },
                new AssetPrice { Price = 100, Quantity = 5 }
            });

        var request = new SimulationRequestDto
        {
            Operation = "naoevendanemcompra",
            Quantity = 10,
            Asset = "BTC/USD"
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.SimulatePriceAsync(request));
    }
}