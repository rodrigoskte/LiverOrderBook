using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LiveOrderBook.Domain.Entities;
using LiveOrderBook.Domain.Interfaces.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LiveOrderBook.Infrastructure.WebSocket;

public class BitstampWebSocketService : BackgroundService
{
    private readonly ClientWebSocket _webSocket = new ClientWebSocket();
    private readonly string _url = "wss://ws.bitstamp.net";
    private readonly IServiceProvider _serviceProvider;

    public BitstampWebSocketService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var client = new ClientWebSocket();
        await client.ConnectAsync(new Uri("wss://ws.bitstamp.net"), stoppingToken);

        // Subscrição aos canais BTC/USD e ETH/USD
        var btcSubscribeMessage =
            Encoding.UTF8.GetBytes("{\"event\": \"bts:subscribe\", \"data\": {\"channel\": \"live_trades_btcusd\"}}");
        var ethSubscribeMessage =
            Encoding.UTF8.GetBytes("{\"event\": \"bts:subscribe\", \"data\": {\"channel\": \"live_trades_ethusd\"}}");

        await client.SendAsync(btcSubscribeMessage, WebSocketMessageType.Text, true, stoppingToken);
        await client.SendAsync(ethSubscribeMessage, WebSocketMessageType.Text, true, stoppingToken);

        var buffer = new byte[1024 * 4];
        while (!stoppingToken.IsCancellationRequested)
        {
            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            // Processa a mensagem e salva no banco
            await ProcessMessage(message);
        }
    }

    public async Task ProcessMessage(string message)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAssetPriceRepository>();
        using var document = JsonDocument.Parse(message);
        var root = document.RootElement;

        if (root.TryGetProperty("event", out var eventProperty))
        {
            var eventName = eventProperty.GetString();

            if (eventName == "trade" && root.TryGetProperty("data", out var dataProperty))
            {
                var channel = root.GetProperty("channel").GetString();
                var asset = channel == "live_trades_btcusd" ? "BTC/USD" :
                    channel == "live_trades_ethusd" ? "ETH/USD" : null;

                if (asset == null) return;

                var price = dataProperty.GetProperty("price").GetDecimal();
                var quantity = dataProperty.GetProperty("amount").GetDecimal();
                var timestamp = DateTime.UtcNow; // Ou extraia do JSON, se disponível

                var assetPrice = new AssetPrice
                {
                    Asset = asset,
                    Price = price,
                    Quantity = quantity,
                    Timestamp = timestamp
                };

                var existingRecords =
                    await repository.GetTradesAsync(assetPrice.Asset, assetPrice.Timestamp.AddSeconds(-5));
                if (existingRecords.Any(r => r.Price == assetPrice.Price && r.Quantity == assetPrice.Quantity))
                {
                    Console.WriteLine("Duplicate trade ignored.");
                    return;
                }

                await repository.SaveAsync(assetPrice);
            }
            else
            {
                Console.WriteLine("Event is not a trade or data is missing.");
            }
        }
        else
        {
            Console.WriteLine("Message does not contain an 'event' property.");
        }
    }

    public async Task ConnectAsync()
    {
        await _webSocket.ConnectAsync(new Uri(_url), CancellationToken.None);
        Console.WriteLine("Connected to Bitstamp WebSocket");

        // Subscribing to channels
        await Subscribe("live_trades_btcusd");
        await Subscribe("live_trades_ethusd");
    }
    
    private async Task Subscribe(string channel)
    {
        var message = new
        {
            @event = "bts:subscribe",
            data = new { channel }
        };

        var jsonMessage = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(jsonMessage);
        await _webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task ListenAsync(Action<string> onMessage)
    {
        var buffer = new byte[4096];
        while (_webSocket.State == WebSocketState.Open)
        {
            var result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            onMessage(message);
        }
    }
}