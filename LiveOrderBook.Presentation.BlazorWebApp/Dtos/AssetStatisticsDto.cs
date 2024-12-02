namespace LiveOrderBook.Presentation.BlazorWebApp.Dtos;

public class AssetStatisticsDto
{
    public string Asset { get; set; } // Nome do ativo, ex: "BTC/USD"
    public decimal HighestPrice { get; set; } // Maior preço
    public decimal LowestPrice { get; set; } // Menor preço
    public decimal AveragePrice { get; set; } // Média de preço no período
    public decimal AverageQuantity { get; set; } // Média de quantidade negociada no período
}