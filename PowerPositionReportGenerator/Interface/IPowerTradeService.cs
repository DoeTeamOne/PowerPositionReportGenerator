using Axpo;

namespace PowerPositionReportGenerator.Interface
{
    public interface IPowerTradeService
    {
        Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date);
    }
}
