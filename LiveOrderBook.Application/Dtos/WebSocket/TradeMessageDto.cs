using System;

namespace LiveOrderBook.Application.Dtos.WebSocket;

public class TradeMessageDto
{
    public string Asset { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public DateTime Timestamp { get; set; }
}