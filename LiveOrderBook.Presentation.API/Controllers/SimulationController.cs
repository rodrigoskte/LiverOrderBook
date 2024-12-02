using System.Threading.Tasks;
using LiveOrderBook.Application.Dtos;
using LiveOrderBook.Application.Interfaces.Services.PriceSimulator;
using LiveOrderBook.Application.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LiveOrderBook.Presentation.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SimulationController : ControllerBase
{
    private readonly IPriceSimulationService _simulationService;
    private readonly ILogger<SimulationController> _logger;
    
    public SimulationController(
        IPriceSimulationService simulationService,
        ILogger<SimulationController> logger)
    {
        _simulationService = simulationService;
        _logger = logger;
    }

    [HttpPost("simulate-price")]
    public async Task<IActionResult> SimulatePrice([FromBody] SimulationRequestDto request)
    {
        _logger.LogInformation("Simulação de preço iniciada para {Asset} com quantidade {Quantity}",
            request.Asset, request.Quantity);
        
        var response = await _simulationService.SimulatePriceAsync(request);
        return Ok(new ResultViewModel<SimulationResponseDto>(response, StatusCodes.Status200OK));
    }
}