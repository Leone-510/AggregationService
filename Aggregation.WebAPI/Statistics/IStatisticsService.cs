namespace Aggregation.WebAPI.Statistics
{
    public interface IStatisticsService
    {
        void LogRequest(string apiName, double responseTimeMs);
        Dictionary<string, ApiStatisticsDto> GetStatistics();
    }
}
