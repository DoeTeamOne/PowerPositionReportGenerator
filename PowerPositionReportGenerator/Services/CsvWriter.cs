using PowerPositionReportGenerator.Interface;
using System.Text;

namespace PowerPositionReportGenerator.Services
{
    public class CsvWriter : ICsvWriter
    {
        public void Write(string folder, Dictionary<int, double> data, DateTime extractTimeLocal)
        {
            _ = Directory.CreateDirectory(folder);

            var fileName = $"PowerPosition_{extractTimeLocal:yyyyMMdd_HHmm}.csv";
            var fullPath = Path.Combine(folder, fileName);

            var sb = new StringBuilder();
            _ = sb.AppendLine("Local Time,Volume");

            for (int period = 1; period <= 24; period++)
            {
                string time = GetLocalTime(period);
                double volume = data.ContainsKey(period) ? data[period] : 0;
                _ = sb.AppendLine($"{time},{volume}");
            }

            File.WriteAllText(fullPath, sb.ToString());
        }

        private string GetLocalTime(int period)
        {
            var baseTime = new TimeSpan(23, 0, 0);
            var offset = TimeSpan.FromHours(period - 1);
            var time = baseTime.Add(offset);
            time = TimeSpan.FromHours(time.TotalHours % 24);
            return $"{(int)time.TotalHours:00}:00";
        }
    }
}
