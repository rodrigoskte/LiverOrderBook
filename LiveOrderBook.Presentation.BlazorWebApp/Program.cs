using LiveOrderBook.Presentation.BlazorWebApp;
using LiveOrderBook.Presentation.BlazorWebApp.Configuration;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using RestSharp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
ConfigureInjections(builder);
builder.Services.AddMudServices();

await builder.Build().RunAsync();

void ConfigureInjections(WebAssemblyHostBuilder webAssemblyHostBuilder)
{
    var baseUrlConfig = new BaseUrlConfiguration();
    builder.Configuration.GetSection(BaseUrlConfiguration.CONFIG_NAME).Bind(baseUrlConfig);
    webAssemblyHostBuilder.Services.AddSingleton(baseUrlConfig);
    webAssemblyHostBuilder.Services.AddSingleton<IRestClient>(sp => new RestClient(baseUrlConfig.ApiBase));
}