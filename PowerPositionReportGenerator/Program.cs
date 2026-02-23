using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PowerPositionReportGenerator.Interface;
using PowerPositionReportGenerator.Services;

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        _ = config.AddJsonFile("appsettings.json", optional: false);
        _ = config.AddCommandLine(args);
    })
    .ConfigureServices((context, services) =>
    {
        _ = services.AddScoped<IExtractService, ExtractService>();
        _ = services.AddScoped<IPowerTradeService, PowerTradeService>();
        _ = services.AddScoped<ITradeAggregator, TradeAggregator>();
        _ = services.AddScoped<ICsvWriter, CsvWriter>();

        _ = services.AddHostedService<PowerPositionScheduler>();
    })
    .ConfigureLogging(logging =>
    {
        _ = logging.AddConsole();
    })
    .RunConsoleAsync();