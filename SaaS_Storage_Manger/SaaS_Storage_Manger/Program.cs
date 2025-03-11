using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SaaS_Storage_Manger.Orchestrator;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<StorageOrchestrator>();
        services.AddLogging();
    })
    .Build();
    
var orchestrator = builder.Services.GetRequiredService<StorageOrchestrator>();
await orchestrator.CreateStorageContainerAsync();

Console.WriteLine("Storage container created successfully");