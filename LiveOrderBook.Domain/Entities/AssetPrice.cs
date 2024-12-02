using System;

namespace LiveOrderBook.Domain.Entities;

public class AssetPrice : BaseEntity
{
    public int Id { get; set; } // Identificador único
    public string Asset { get; set; } // Ex.: BTC/USD ou ETH/USD
    public decimal Price { get; set; } // Preço do trade
    public decimal Quantity { get; set; } // Quantidade negociada
    public DateTime Timestamp { get; set; } // Momento do trade
}