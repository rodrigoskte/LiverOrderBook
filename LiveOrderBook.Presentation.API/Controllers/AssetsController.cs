using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveOrderBook.Application.Constants;
using LiveOrderBook.Application.Dtos.WebSocket;
using LiveOrderBook.Application.Interfaces.Services.WebSocket;
using LiveOrderBook.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LiveOrderBook.Presentation.API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/v1/[controller]/")]
public class AssetsController : ControllerBase
{
    private readonly IAssetStatisticsService _statisticsService;
    private readonly ITradeProcessorService _processorService;
    private readonly ILogger<SimulationController> _logger;

    public AssetsController(
        IAssetStatisticsService statisticsService,
        ITradeProcessorService processorService,
        ILogger<SimulationController> logger)
    {
        _statisticsService = statisticsService;
        _processorService = processorService;
        _logger = logger;
    }
    
    /// <summary>
    /// Processa um trade manualmente.
    /// </summary>
    [HttpPost("process-trade")]
    public async Task<IActionResult> ProcessTrade([FromBody] TradeMessageDto tradeDto)
    {
        _logger.LogInformation("Processo de Trade iniciado {Asset} com o preço {Price} com quantidade {Quantity}",
            tradeDto.Asset, tradeDto.Price, tradeDto.Quantity);
        
        await _processorService.ProcessTradeAsync(tradeDto);
        return Ok(new { Message = "Trade processed successfully" });
    }
    
    /// <summary>
    /// Retorna as estatísticas de um ativo.
    /// </summary>
    [HttpGet("statistics/{asset}")]
    public async Task<IActionResult> GetStatistics(string asset)
    {
        _logger.LogInformation("Processo de buscar estatística iniciado: {Asset}",
            asset);
        
        var since = DateTime.UtcNow.AddSeconds(-5);
        asset = Uri.UnescapeDataString(asset);
        since = since.ToUniversalTime();
        var statistics = await _statisticsService.GetStatisticsAsync(asset, since);

        if (statistics == null)
            return NotFound(new ResultViewModel<string>($"{MessageConstants.StatisticsNotFound} para este asset: {asset}" , StatusCodes.Status404NotFound));

        return Ok(new ResultViewModel<AssetStatisticsDto>(statistics, StatusCodes.Status200OK));
    }
    
    /// <summary>
    /// Retorna todas as estatísticas.
    /// </summary>
    [HttpGet("all-statistics")]
    public async Task<IActionResult> GetAllStatistics()
    {
        _logger.LogInformation("Processo de buscar todas as estatística iniciado");
        
        var since = DateTime.UtcNow.AddSeconds(-5);
        since = since.ToUniversalTime();
        var statistics = await _statisticsService.GetAllStatisticsAsync(since);

        if (!statistics.Any())
            return NotFound(new ResultViewModel<string>(MessageConstants.StatisticsNotFound, StatusCodes.Status404NotFound));

        return Ok(new ResultViewModel<List<AssetStatisticsDto>>(statistics, StatusCodes.Status200OK));
    }
}