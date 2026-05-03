namespace TaskNest.API.Interfaces
{
    public interface IActivityLogService
    {
        Task<IEnumerable<ActivityLog>> GetActivitiesByTaskIdAsync(Guid taskId);
        Task<ActivityLog> CreateActivityAsync(ActivityLog activity);
        Task LogTaskCreatedAsync(Guid taskId, string userId);
        Task LogTaskMovedAsync(Guid taskId, string userId, string fromColumn, string toColumn);
        Task LogTaskUpdatedAsync(Guid taskId, string userId, string details);
    }
}