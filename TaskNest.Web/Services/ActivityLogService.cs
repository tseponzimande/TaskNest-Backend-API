namespace TaskNest.API.Services
{
    public class ActivityLogService(AppDbContext context) : IActivityLogService
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<ActivityLog>> GetActivitiesByTaskIdAsync(Guid taskId)
        {
            try
            {
                return await _context.ActivityLogs
                    .Include(a => a.User)
                    .Where(a => a.TaskItemId == taskId)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
            }
            catch
            {
                return Enumerable.Empty<ActivityLog>();
            }
        }

        public async Task<ActivityLog> CreateActivityAsync(ActivityLog activity)
        {
            try
            {
                activity.Id = Guid.NewGuid();
                activity.CreatedAt = DateTime.UtcNow;

                await _context.ActivityLogs.AddAsync(activity);
                await _context.SaveChangesAsync();

                return activity;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating activity log", ex);
            }
        }

        public async Task LogTaskCreatedAsync(Guid taskId, string userId)
        {
            try
            {

                var activity = new ActivityLog
                {
                    TaskItemId = taskId,
                    ApplicationUserId = userId,
                    Action = "Created",
                    Details = "Task was created"
                };
                await CreateActivityAsync(activity);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}", ex);
            }
        }

        public async Task LogTaskMovedAsync(Guid taskId, string userId, string fromColumn, string toColumn)
        {
            try
            {
                var activity = new ActivityLog
                {
                    TaskItemId = taskId,
                    ApplicationUserId = userId,
                    Action = "Moved",
                    Details = $"Moved from '{fromColumn}' to '{toColumn}'"
                };
                await CreateActivityAsync(activity);
            }
            catch (Exception ex)
            {
                throw new Exception($": {ex.Message}", ex);
            }
        }

        public async Task LogTaskUpdatedAsync(Guid taskId, string userId, string details)
        {
            try
            {
                var activity = new ActivityLog
                {
                    TaskItemId = taskId,
                    ApplicationUserId = userId,
                    Action = "Updated",
                    Details = details
                };
                await CreateActivityAsync(activity);
            }
            catch (Exception ex)
            {
                throw new Exception($": {ex.Message}", ex);
            }
        }
    }
}
