using Aggregation.WebAPI.Statistics;
using Microsoft.AspNetCore.Mvc;

namespace Aggregation.WebAPI.Controllers
{
    [ApiController]
    [Route("api/statistics")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statsService;

        public StatisticsController(IStatisticsService statsService)
        {
            _statsService = statsService;
        }

        [HttpGet]
        public IActionResult GetRequestStatistics()
        {
            var stats = _statsService.GetStatistics();
            return Ok(stats);
        }
    }
}
