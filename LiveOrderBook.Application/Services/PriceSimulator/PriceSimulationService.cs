using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveOrderBook.Application.Dtos;
using LiveOrderBook.Application.Interfaces.Services.PriceSimulator;
using LiveOrderBook.Domain.Interfaces.Repository;

namespace LiveOrderBook.Application.Services.PriceSimulator;

public class PriceSimulationService : IPriceSimulationService
{
    private readonly IAssetPriceRepository _assetPriceRepository;

    public PriceSimulationService(IAssetPriceRepository assetPriceRepository)
    {
        _assetPriceRepository = assetPriceRepository;
    }
    
    public async Task<SimulationResponseDto> SimulatePriceAsync(SimulationRequestDto request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Operation) || request.Quantity <= 0)
            throw new ArgumentException("Dados inválidos para simulação.");

        var trades = await _assetPriceRepository.GetTradesAsync(request.Asset, DateTime.UtcNow.AddSeconds(-5));

        if (trades == null || !trades.Any())
            throw new KeyNotFoundException($"Nenhuma informação encontrada para o ativo {request.Asset}.");

        List<TradePriceDetailDto> usedTrades = new();
        decimal totalPrice = 0;
        decimal remainingQuantity = request.Quantity;

        if (request.Operation.Equals("compra", StringComparison.OrdinalIgnoreCase))
        {
            var asks = trades.OrderBy(t => t.Price).ToList();
            foreach (var ask in asks)
            {
                if (remainingQuantity <= 0)
                    break;

                var quantityUsed = Math.Min(remainingQuantity, ask.Quantity);
                totalPrice += quantityUsed * ask.Price;
                remainingQuantity -= quantityUsed;

                usedTrades.Add(new TradePriceDetailDto
                {
                    Price = ask.Price,
                    Quantity = quantityUsed
                });
            }
        }
        else if (request.Operation.Equals("venda", StringComparison.OrdinalIgnoreCase))
        {
            var bids = trades.OrderByDescending(t => t.Price).ToList();
            foreach (var bid in bids)
            {
                if (remainingQuantity <= 0)
                    break;

                var quantityUsed = Math.Min(remainingQuantity, bid.Quantity);
                totalPrice += quantityUsed * bid.Price;
                remainingQuantity -= quantityUsed;

                usedTrades.Add(new TradePriceDetailDto
                {
                    Price = bid.Price,
                    Quantity = quantityUsed
                });
            }
        }
        else
        {
            throw new ArgumentException("Operação inválida. Use 'compra' ou 'venda'.");
        }

        if (remainingQuantity > 0)
            throw new InvalidOperationException("Não foi possível atender a quantidade total solicitada com os dados disponíveis.");

        return new SimulationResponseDto
        {
            Id = Guid.NewGuid().ToString(),
            Asset = request.Asset,
            Operation = request.Operation,
            RequestedQuantity = request.Quantity,
            TotalPrice = totalPrice,
            UsedTrades = usedTrades
        };
    }
}