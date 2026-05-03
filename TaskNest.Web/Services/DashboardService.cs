namespace TaskNest.API.Services
{
    public class DashboardService(AppDbContext context, ILogger<DashboardService> logger) : IDashboardService
    {
        private readonly AppDbContext _context = context;
        private readonly ILogger<DashboardService> _logger = logger;

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(string userId)
        {
            try
            {
                var userBoardIds = await _context.BoardUsers
                    .Where(bu => bu.ApplicationUserId == userId)
                    .Select(bu => bu.BoardId)
                    .ToListAsync();

                var allTasks = await _context.Tasks
                    .Where(t => userBoardIds.Contains(t.BoardId))
                    .ToListAsync();

                var completedColumnIds = await _context.BoardColumns
                    .Where(c => c.Name.ToLower().Contains(DashboardStatus.done.ToString()) ||
                                c.Name.ToLower().Contains(DashboardStatus.complete.ToString()))
                    .Select(c => c.Id)
                    .ToListAsync();

                var now = DateTime.UtcNow;

                return new DashboardStatsDto
                {
                    TotalBoards = await _context.Boards.CountAsync(),
                    TotalTasks = await _context.Tasks.CountAsync(),
                    CompletedTasks = allTasks.Count(t => t.ColumnId.HasValue && completedColumnIds.Contains(t.ColumnId.Value)),
                    OverdueTasks = allTasks.Count(t => t.DueDate.HasValue && t.DueDate.Value < now),
                    TasksDueThisWeek = allTasks.Count(t => t.DueDate.HasValue && t.DueDate.Value >= now && t.DueDate.Value <= now.AddDays(7)),
                    ActiveBoards = userBoardIds.Count,
                    TotalUsers = await _context.Users.CountAsync(),
                    MyBoards = userBoardIds.Count,
                    MyTasks = allTasks.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetDashboardStatsAsync");
                return new DashboardStatsDto();
            }
        }

        public async Task<IEnumerable<TasksByStatusDto>> GetTasksByStatusAsync(string userId)
        {
            try
            {
                var userBoardIds = await _context.BoardUsers
                    .Where(bu => bu.ApplicationUserId == userId)
                    .Select(bu => bu.BoardId)
                    .ToListAsync();

                var tasks = await _context.Tasks
                    .Where(t => userBoardIds.Contains(t.BoardId))
                    .ToListAsync();

                var now = DateTime.UtcNow;

                var stats = new List<TasksByStatusDto>
                {
                    new TasksByStatusDto { Status = Enums.TaskStatus.OnTime.ToString(), Count = tasks.Count(t => !t.DueDate.HasValue || t.DueDate.Value >= now), Color = "#4caf50" },
                    new TasksByStatusDto { Status = Enums.TaskStatus.Overdue.ToString(), Count = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value < now), Color = "#f44336" },
                    new TasksByStatusDto { Status = Enums.TaskStatus.DueThisWeek.ToString(), Count = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value >= now && t.DueDate.Value <= now.AddDays(7)), Color = "#ff9800" }
                };

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTasksByStatusAsync");
                return Enumerable.Empty<TasksByStatusDto>();
            }
        }

        public async Task<IEnumerable<TasksByColumnDto>> GetTasksByColumnAsync(string userId)
        {
            try
            {
                var userBoardIds = await _context.BoardUsers
                    .Where(bu => bu.ApplicationUserId == userId)
                    .Select(bu => bu.BoardId)
                    .ToListAsync();

                var result = await _context.Tasks
                    .Where(t => userBoardIds.Contains(t.BoardId) && t.ColumnId.HasValue)
                    .GroupBy(t => new { ColumnName = t.Column!.Name, BoardName = t.Board!.Name })
                    .Select(g => new TasksByColumnDto
                    {
                        ColumnName = g.Key.ColumnName,
                        BoardName = g.Key.BoardName,
                        TaskCount = g.Count()
                    })
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTasksByColumnAsync");
                return Enumerable.Empty<TasksByColumnDto>();
            }
        }


        public async Task<IEnumerable<RecentActivityDto>> GetRecentActivityAsync(string userId, int count = 10)
        {
            try
            {
                var userBoardIds = await _context.BoardUsers
                    .Where(bu => bu.ApplicationUserId == userId)
                    .Select(bu => bu.BoardId)
                    .ToListAsync();

                var activities = await _context.ActivityLogs
                    .Include(a => a.User)
                    .Include(a => a.TaskItem)
                    .ThenInclude(t => t!.Board)
                    .Where(a => userBoardIds.Contains(a.TaskItem!.BoardId))
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(count)
                    .Select(a => new RecentActivityDto
                    {
                        Action = a.Action,
                        Description = a.Details ?? "",
                        UserEmail = a.User!.Email ?? "",
                        Timestamp = a.CreatedAt,
                        BoardName = a.TaskItem!.Board!.Name
                    })
                    .ToListAsync();

                return activities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRecentActivityAsync");
                return Enumerable.Empty<RecentActivityDto>();
            }
        }

        public async Task<IEnumerable<BoardStatsDto>> GetBoardStatsAsync(string userId)
        {
            try
            {
                var userBoardIds = await _context.BoardUsers
                    .Where(bu => bu.ApplicationUserId == userId)
                    .Select(bu => bu.BoardId)
                    .ToListAsync();

                var completedColumnIds = await _context.BoardColumns
                    .Where(c => c.Name.ToLower().Contains(DashboardStatus.done.ToString()) ||
                                c.Name.ToLower().Contains(DashboardStatus.complete.ToString()))
                    .Select(c => c.Id)
                    .ToListAsync();

                var boardStats = await _context.Boards
                    .Where(b => userBoardIds.Contains(b.Id))
                    .Select(b => new BoardStatsDto
                    {
                        BoardId = b.Id,
                        BoardName = b.Name,
                        TaskCount = b.Tasks.Count,
                        CompletedTasks = b.Tasks.Count(t => t.ColumnId.HasValue && completedColumnIds.Contains(t.ColumnId.Value)),
                        MemberCount = b.BoardUsers.Count
                    })
                    .ToListAsync();

                return boardStats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetBoardStatsAsync");
                return Enumerable.Empty<BoardStatsDto>();
            }
        }

        public async Task<IEnumerable<TaskTrendDto>> GetTaskTrendAsync(string userId, int days = 7)
        {
            try
            {
                var userBoardIds = await _context.BoardUsers
                    .Where(bu => bu.ApplicationUserId == userId)
                    .Select(bu => bu.BoardId)
                    .ToListAsync();

                var startDate = DateTime.UtcNow.AddDays(-days).Date;
                var tasks = await _context.Tasks
                    .Where(t => userBoardIds.Contains(t.BoardId) && t.CreatedAt >= startDate)
                    .ToListAsync();

                var completedColumnIds = await _context.BoardColumns
                    .Where(c => c.Name.ToLower().Contains(DashboardStatus.done.ToString()) ||
                                c.Name.ToLower().Contains(DashboardStatus.complete.ToString()))
                    .Select(c => c.Id)
                    .ToListAsync();

                var trend = Enumerable.Range(0, days)
                    .Select(i => startDate.AddDays(i))
                    .Select(date => new TaskTrendDto
                    {
                        Date = date.ToString("MMM dd"),
                        Created = tasks.Count(t => t.CreatedAt.Date == date),
                        Completed = tasks.Count(t => t.ColumnId.HasValue && completedColumnIds.Contains(t.ColumnId.Value) && t.CreatedAt.Date == date)
                    })
                    .ToList();

                return trend;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTaskTrendAsync");
                return Enumerable.Empty<TaskTrendDto>();
            }
        }
    }
}
