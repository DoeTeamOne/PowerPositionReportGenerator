using Axpo;
using PowerPositionReportGenerator.Interface;

namespace PowerPositionReportGenerator.Services
{
    public class PowerTradeService : IPowerTradeService
    {
        private readonly PowerService _service = new();

        public Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date)
            => _service.GetTradesAsync(date);
    }
}
