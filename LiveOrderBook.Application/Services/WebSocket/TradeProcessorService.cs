using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using LiveOrderBook.Application.Dtos.WebSocket;
using LiveOrderBook.Application.Interfaces.Services.WebSocket;
using LiveOrderBook.Domain.Entities;
using LiveOrderBook.Domain.Interfaces.Repository;

namespace LiveOrderBook.Application.Services.WebSocket;

public class TradeProcessorService : ITradeProcessorService
{
    private readonly IAssetPriceRepository _assertPriceRepository;
    private readonly IValidator<AssetPrice> _assetPriceValidator;

    public TradeProcessorService(
        IAssetPriceRepository assertPriceRepository, 
        IValidator<AssetPrice> assetPriceValidator)
    {
        _assertPriceRepository = assertPriceRepository;
        _assetPriceValidator = assetPriceValidator;
    }

    public async Task ProcessTradeAsync(TradeMessageDto tradeDto)
    {
        var assetPrice = new AssetPrice
        {
            Asset = tradeDto.Asset,
            Price = tradeDto.Price,
            Quantity = tradeDto.Quantity,
            Timestamp = tradeDto.Timestamp
        };
        
        var validationResult = _assetPriceValidator.Validate(assetPrice);
        
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException($"Validation failed: {errors}");
        }
        
        await _assertPriceRepository.SaveAsync(assetPrice);
    }
}