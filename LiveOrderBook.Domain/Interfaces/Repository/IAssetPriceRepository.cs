using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveOrderBook.Domain.Entities;

namespace LiveOrderBook.Domain.Interfaces.Repository;

public interface IAssetPriceRepository
{
    Task SaveAsync(AssetPrice assetPrice);
    Task<List<AssetPrice>> GetTradesAsync(string asset, DateTime since);
}