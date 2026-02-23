using Axpo;

namespace PowerPositionReportGenerator.Interface
{
    public interface ITradeAggregator
    {
        Dictionary<int, double> Aggregate(IEnumerable<PowerTrade> trades);
    }
}
