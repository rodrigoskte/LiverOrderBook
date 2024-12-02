using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveOrderBook.Application.Dtos.WebSocket;

namespace LiveOrderBook.Application.Interfaces.Services.WebSocket;

public interface IAssetStatisticsService
{
    Task<AssetStatisticsDto> GetStatisticsAsync(string asset, DateTime since);
    Task<List<AssetStatisticsDto>> GetAllStatisticsAsync(DateTime since);
}