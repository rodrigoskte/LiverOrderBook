namespace LiveOrderBook.Application.Dtos;

public class SimulationRequestDto
{
    public string Operation { get; set; } // "compra" ou "venda"
    public string Asset { get; set; } // Ex: "BTC/USD"
    public decimal Quantity { get; set; } // Quantidade total desejada
}