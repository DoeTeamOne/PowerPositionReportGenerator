using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PowerPositionReportGenerator.Interface;

namespace PowerPositionReportGenerator.Services
{
    public class PowerPositionScheduler : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<PowerPositionScheduler> _logger;

        public PowerPositionScheduler(
            IServiceScopeFactory scopeFactory,
            IConfiguration config,
            ILogger<PowerPositionScheduler> logger)
        {
            _scopeFactory = scopeFactory;
            _config = config;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int interval = int.Parse(_config["ExtractIntervalMinutes"]);

            await RunExtractAsync();

            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(interval));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await RunExtractAsync();
            }
        }

        private async Task RunExtractAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var extractService = scope.ServiceProvider
                .GetRequiredService<IExtractService>();

            await extractService.RunAsync();
        }
    }
}
