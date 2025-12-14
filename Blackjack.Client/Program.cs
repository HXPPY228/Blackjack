using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blackjack.Client;
using Blackjack.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Регистрируем наш мок-сервис
builder.Services.AddScoped<IGameHubService, MockGameHubService>();

// Настраиваем корневой компонент
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HTTP клиент
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Собираем и запускаем
await builder.Build().RunAsync();