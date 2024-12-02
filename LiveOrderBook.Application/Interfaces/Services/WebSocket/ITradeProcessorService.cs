using System.Threading.Tasks;
using LiveOrderBook.Application.Dtos.WebSocket;

namespace LiveOrderBook.Application.Interfaces.Services.WebSocket;

public interface ITradeProcessorService
{
    Task ProcessTradeAsync(TradeMessageDto tradeDto);
}