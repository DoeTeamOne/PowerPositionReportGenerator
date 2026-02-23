using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PowerPositionReportGenerator.Interface;

namespace PowerPositionReportGenerator.Services
{
    public class ExtractService : IExtractService
    {
        private readonly IPowerTradeService _powerTradeService;
        private readonly ITradeAggregator _aggregator;
        private readonly ICsvWriter _writer;
        private readonly IConfiguration _config;
        private readonly ILogger<ExtractService> _logger;

        public ExtractService(
            IPowerTradeService powerService,
            ITradeAggregator aggregator,
            ICsvWriter writer,
            IConfiguration config,
            ILogger<ExtractService> logger)
        {
            _powerTradeService = powerService;
            _aggregator = aggregator;
            _writer = writer;
            _config = config;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var timeZone = GetLondonTimeZone();
            var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            _logger.LogInformation("Starting extract for {Date}", localNow);

            var trades = await _powerTradeService.GetTradesAsync(localNow.Date);// calls the wrapper of dll service


            var aggregated = _aggregator.Aggregate(trades);

            string folder = _config["OutputFolder"];

            _writer.Write(folder, aggregated, localNow);

            _logger.LogInformation("Extract completed successfully");
        }

        private TimeZoneInfo GetLondonTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
            }
            catch
            {
                return TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            }
        }
    }
}
