namespace TaskNest.API.Controllers
{
    [Authorize]
    //[AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController(IDashboardService dashboardService) : ControllerBase
    {
        private readonly IDashboardService _dashboardService = dashboardService;

        [HttpGet("stats")]
        [ProducesResponseType(Status200OK)]
        public async Task<ActionResult<DashboardStatsDto>> GetStats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var stats = await _dashboardService.GetDashboardStatsAsync(userId);
            return Ok(stats);
        }

        [HttpGet("tasks-by-status")]
        [ProducesResponseType(Status200OK)]
        public async Task<ActionResult<IEnumerable<TasksByStatusDto>>> GetTasksByStatus()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var data = await _dashboardService.GetTasksByStatusAsync(userId);
            return Ok(data);
        }

        [HttpGet("tasks-by-column")]
        [ProducesResponseType(Status200OK)]
        public async Task<ActionResult<IEnumerable<TasksByColumnDto>>> GetTasksByColumn()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var data = await _dashboardService.GetTasksByColumnAsync(userId);
            return Ok(data);
        }

        [HttpGet("recent-activity")]
        [ProducesResponseType(Status200OK)]
        public async Task<ActionResult<IEnumerable<RecentActivityDto>>> GetRecentActivity([FromQuery] int count = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var activities = await _dashboardService.GetRecentActivityAsync(userId, count);
            return Ok(activities);
        }

        [HttpGet("board-stats")]
        [ProducesResponseType(Status200OK)]
        public async Task<ActionResult<IEnumerable<BoardStatsDto>>> GetBoardStats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var stats = await _dashboardService.GetBoardStatsAsync(userId);
            return Ok(stats);
        }

        [HttpGet("task-trend")]
        [ProducesResponseType(Status200OK)]
        public async Task<ActionResult<IEnumerable<TaskTrendDto>>> GetTaskTrend([FromQuery] int days = 7)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var trend = await _dashboardService.GetTaskTrendAsync(userId, days);
            return Ok(trend);
        }
    }
}