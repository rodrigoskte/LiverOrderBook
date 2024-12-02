using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveOrderBook.Application.Dtos.WebSocket;
using LiveOrderBook.Application.Interfaces.Services.WebSocket;
using LiveOrderBook.Domain.Interfaces.Repository;

namespace LiveOrderBook.Application.Services.WebSocket;

public class AssetStatisticsService : IAssetStatisticsService
{
    private readonly IAssetPriceRepository _assertPriceRepository;

    public AssetStatisticsService(IAssetPriceRepository assertPriceRepository)
    {
        _assertPriceRepository = assertPriceRepository;
    }

    public async Task<AssetStatisticsDto> GetStatisticsAsync(string asset, DateTime since)
    {
        // Certifique-se de que GetTradesAsync retorna uma lista de AssetPrice
        var trades = await _assertPriceRepository.GetTradesAsync(asset, since);

        return new AssetStatisticsDto
        {
            HighestPrice = trades.Max(t => t.Price),
            LowestPrice = trades.Min(t => t.Price),
            AveragePrice = trades.Average(t => t.Price),
            AverageQuantity = trades.Average(t => t.Quantity)
        };
    }
    
    public async Task<List<AssetStatisticsDto>> GetAllStatisticsAsync(DateTime since)
    {
        var assets = new[] { "BTC/USD", "ETH/USD" };
        var statistics = new List<AssetStatisticsDto>();

        foreach (var asset in assets)
        {
            var trades = await _assertPriceRepository.GetTradesAsync(asset, since);

            if (trades.Any())
            {
                statistics.Add(new AssetStatisticsDto
                {
                    Asset = asset,
                    HighestPrice = trades.Max(t => t.Price),
                    LowestPrice = trades.Min(t => t.Price),
                    AveragePrice = trades.Average(t => t.Price),
                    AverageQuantity = trades.Average(t => t.Quantity)
                });
            }
        }

        return statistics;
    }
}