using Axpo;
using PowerPositionReportGenerator.Interface;

namespace PowerPositionReportGenerator.Services
{
    public class TradeAggregator : ITradeAggregator
    {
        public Dictionary<int, double> Aggregate(IEnumerable<PowerTrade> trades)
        {
            var result = new Dictionary<int, double>();

            foreach (var trade in trades)
            {
                foreach (var period in trade.Periods)
                {
                    if (!result.ContainsKey(period.Period))
                        result[period.Period] = 0;

                    result[period.Period] += period.Volume;
                }
            }

            return result;
        }
    }
}
