namespace PowerPositionReportGenerator.Interface
{
    public interface ICsvWriter
    {
        void Write(string folder, Dictionary<int, double> data, DateTime extractTimeLocal);
    }
}
