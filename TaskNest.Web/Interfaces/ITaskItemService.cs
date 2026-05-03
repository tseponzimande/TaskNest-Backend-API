namespace TaskNest.API.Interfaces
{
    public interface ITaskItemService
    {
        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<TaskItem?> GetByIdAsync(Guid id);
        Task<IEnumerable<TaskItem>> GetByColumnAsync(Guid columnId);
        Task<TaskItem> CreateAsync(TaskItem taskItem, string userId);
        Task<bool> UpdateAsync(TaskItem taskItem, string userId);
        Task<bool> DeleteAsync(Guid taskId, string userId);
        Task<bool> MoveTaskAsync(Guid taskId, Guid newColumnId, int newPosition, string userId);
    }
}