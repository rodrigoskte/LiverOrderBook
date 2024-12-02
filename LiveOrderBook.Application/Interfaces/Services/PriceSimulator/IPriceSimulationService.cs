using System.Threading.Tasks;
using LiveOrderBook.Application.Dtos;

namespace LiveOrderBook.Application.Interfaces.Services.PriceSimulator;

public interface IPriceSimulationService
{
    Task<SimulationResponseDto> SimulatePriceAsync(SimulationRequestDto request);
}