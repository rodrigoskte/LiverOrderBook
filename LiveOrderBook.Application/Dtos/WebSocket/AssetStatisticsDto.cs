namespace LiveOrderBook.Application.Dtos.WebSocket;

public class AssetStatisticsDto
{
    public string Asset { get; set; }
    public decimal HighestPrice { get; set; }
    public decimal LowestPrice { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal AverageQuantity { get; set; }
}