using System;

namespace LiveOrderBook.Domain.Entities;

public class TradeMessage : BaseEntity
{
    public string Asset { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public DateTime Timestamp { get; set; }
}