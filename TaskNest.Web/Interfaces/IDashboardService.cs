namespace TaskNest.API.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync(string userId);
        Task<IEnumerable<TasksByStatusDto>> GetTasksByStatusAsync(string userId);
        Task<IEnumerable<TasksByColumnDto>> GetTasksByColumnAsync(string userId);
        Task<IEnumerable<RecentActivityDto>> GetRecentActivityAsync(string userId, int count = 10);
        Task<IEnumerable<BoardStatsDto>> GetBoardStatsAsync(string userId);
        Task<IEnumerable<TaskTrendDto>> GetTaskTrendAsync(string userId, int days = 7);
    }
}
