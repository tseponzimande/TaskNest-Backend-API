namespace TaskNest.API.Interfaces
{
    public interface INotificationService
    {
        Task NotifyTaskCreatedAsync(Guid boardId, string taskTitle, string createdBy);
        Task NotifyTaskUpdatedAsync(Guid boardId, string taskTitle, string updatedBy);
        Task NotifyTaskMovedAsync(Guid boardId, string taskTitle, string movedBy, string fromColumn, string toColumn);
        Task NotifyTaskDeletedAsync(Guid boardId, string taskTitle, string deletedBy);
        Task NotifyCommentAddedAsync(Guid boardId, string taskTitle, string commentedBy);
        Task NotifyColumnCreatedAsync(Guid boardId, string columnName, string createdBy);
        Task NotifyColumnUpdatedAsync(Guid boardId, string columnName, string updatedBy);
        Task NotifyColumnDeletedAsync(Guid boardId, string columnName, string deletedBy);
    }
}