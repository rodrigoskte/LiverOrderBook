using FluentValidation;
using LiveOrderBook.Application.Interfaces.Services.PriceSimulator;
using LiveOrderBook.Application.Interfaces.Services.WebSocket;
using LiveOrderBook.Application.Services.PriceSimulator;
using LiveOrderBook.Application.Services.WebSocket;
using LiveOrderBook.Application.Validators;
using LiveOrderBook.Domain.Interfaces.Repository;
using LiveOrderBook.Infrastructure.Context;
using LiveOrderBook.Infrastructure.Repository;
using LiveOrderBook.Infrastructure.WebSocket;
using LiveOrderBook.Presentation.API.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
ConfigureDbContext(builder);
ConfigureInjection(builder);
builder.Logging.ClearProviders(); 
builder.Logging.AddConsole();    
builder.Logging.AddDebug();      
if (OperatingSystem.IsWindows())
{
    builder.Logging.AddEventLog();
}
builder.Services.AddHealthChecks()
    .AddCheck("API Alive Check", () => HealthCheckResult.Healthy("API está rodando!"))
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultSqlConnection_dev"));

var app = builder.Build();

await ApplyMigrations(app);
app.UseSwagger();
app.UseSwaggerUI();
ConfigureCors(app);
//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapHealthChecks("/health");
app.Run();

static void ConfigureInjection(WebApplicationBuilder webApplicationBuilder)
{
    webApplicationBuilder.Services.AddHostedService<BitstampWebSocketService>();
    webApplicationBuilder.Services.AddScoped<IAssetPriceRepository, AssetPriceRepository>();
    webApplicationBuilder.Services.AddScoped<IPriceSimulationService, PriceSimulationService>();
    webApplicationBuilder.Services.AddScoped<IAssetStatisticsService, AssetStatisticsService>();
    webApplicationBuilder.Services.AddScoped<ITradeProcessorService, TradeProcessorService>();
    
    webApplicationBuilder.Services.AddValidatorsFromAssemblyContaining<AssetPriceValidator>();
}

static void ConfigureDbContext(WebApplicationBuilder builder1)
{
    builder1.Services.AddDbContext<LiveOrderBookApiDbContext>(options =>
    {
        options.UseSqlServer(builder1.Configuration.GetConnectionString("DefaultSqlConnection_dev"),
            sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });
    });
}

static async Task ApplyMigrations(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<LiveOrderBookApiDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}

static void ConfigureCors(WebApplication app)
{
    app.UseCors(builder =>
    {
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
        builder.AllowAnyOrigin();
    });
}