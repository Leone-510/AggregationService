namespace Aggregation.WebAPI.Statistics
{
    public class ApiStatisticsDto
    {
        public int TotalRequests { get; set; }
        public double AverageResponseTimeMs { get; set; }
        public int FastRequestsCount { get; set; }
        public int AverageRequestsCount { get; set; }
        public int SlowRequestsCount { get; set; }
    }
}
