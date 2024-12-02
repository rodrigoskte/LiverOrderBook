using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveOrderBook.Domain.Entities;
using LiveOrderBook.Domain.Interfaces.Repository;
using LiveOrderBook.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace LiveOrderBook.Infrastructure.Repository;

public class AssetPriceRepository : IAssetPriceRepository
{
    private readonly LiveOrderBookApiDbContext _context;

    public AssetPriceRepository(LiveOrderBookApiDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(AssetPrice trade)
    {
        await _context.AssetPrices.AddAsync(trade);
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<AssetPrice>> GetTradesAsync(string asset, DateTime since)
    {
        return await _context.AssetPrices
            .Where(p => p.Asset == asset && p.Timestamp >= since)
            .ToListAsync();
    }
}