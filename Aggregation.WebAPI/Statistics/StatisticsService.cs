using System.Collections.Concurrent;

namespace Aggregation.WebAPI.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ConcurrentDictionary<string, ConcurrentBag<double>> _requestTimes = new();

        public void LogRequest(string apiName, double responseTimeMs)
        {
            _requestTimes.GetOrAdd(apiName, _ => new ConcurrentBag<double>()).Add(responseTimeMs);
        }

        public Dictionary<string, ApiStatisticsDto> GetStatistics()
        {
            var result = new Dictionary<string, ApiStatisticsDto>();

            foreach (var kvp in _requestTimes)
            {
                var times = kvp.Value.ToList();
                if (times.Count == 0)
                    continue;

                result.Add(kvp.Key, new ApiStatisticsDto
                {
                    TotalRequests = times.Count,
                    AverageResponseTimeMs = Math.Round(times.Average(), 2),
                    FastRequestsCount = times.Count(t => t < 100),                  // < 100ms
                    AverageRequestsCount = times.Count(t => t >= 100 && t <= 200),  // 100-200ms
                    SlowRequestsCount = times.Count(t => t > 200)                   // > 200ms
                });
            }

            return result;
        }
    }
}
