using System.Collections.Generic;

namespace LiveOrderBook.Application.Dtos;

public class SimulationResponseDto
{
    public string Id { get; set; } // ID único da simulação
    public string Asset { get; set; } // Ativo ("BTC/USD", "ETH/USD")
    public string Operation { get; set; } // "compra" ou "venda"
    public decimal RequestedQuantity { get; set; } // Quantidade solicitada
    public decimal TotalPrice { get; set; } // Preço total da operação
    public List<TradePriceDetailDto> UsedTrades { get; set; } // Lista de trades usados
}