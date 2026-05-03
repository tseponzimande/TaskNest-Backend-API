namespace TaskNest.API.Services
{
    public class NotificationService(IHubContext<NotificationHub> hubContext) : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;

        public async Task NotifyTaskCreatedAsync(Guid boardId, string taskTitle, string createdBy)
        {
            var notification = new
            {
                Type = "TaskCreated",
                BoardId = boardId,
                Message = $"{createdBy} created task '{taskTitle}'",
                Title = taskTitle,
                CreatedBy = createdBy,
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"board_{boardId}")
                .SendAsync("ReceiveNotification", notification);
        }

        public async Task NotifyTaskUpdatedAsync(Guid boardId, string taskTitle, string updatedBy)
        {
            var notification = new
            {
                Type = "TaskUpdated",
                BoardId = boardId,
                Message = $"{updatedBy} updated task '{taskTitle}'",
                Title = taskTitle,
                UpdatedBy = updatedBy,
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"board_{boardId}")
                .SendAsync("ReceiveNotification", notification);

            await _hubContext.Clients.Group($"board_{boardId}")
                .SendAsync("RefreshBoard", new { BoardId = boardId });
        }

        public async Task NotifyTaskMovedAsync(Guid boardId, string taskTitle, string movedBy, string fromColumn, string toColumn)
        {
            var notification = new
            {
                Type = "TaskMoved",
                BoardId = boardId,
                Message = $"{movedBy} moved '{taskTitle}' from {fromColumn} to {toColumn}",
                Title = taskTitle,
                MovedBy = movedBy,
                FromColumn = fromColumn,
                ToColumn = toColumn,
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"board_{boardId}")
                .SendAsync("ReceiveNotification", notification);

            await _hubContext.Clients.Group($"board_{boardId}")
                .SendAsync("RefreshBoard", new { BoardId = boardId });
        }

        public async Task NotifyTaskDeletedAsync(Guid boardId, string taskTitle, string deletedBy)
        {
            var notification = new
            {
                Type = "TaskDeleted",
                BoardId = boardId,
                Message = $"{deletedBy} deleted task '{taskTitle}'",
                Title = taskTitle,
                DeletedBy = deletedBy,
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"board_{boardId}")
                .SendAsync("ReceiveNotification", notification);

            await _hubContext.Clients.Group($"board_{boardId}")
                .SendAsync("RefreshBoard", new { BoardId = boardId });
        }

        public async Task NotifyCommentAddedAsync(Guid boardId, string taskTitle, string commentedBy)
        {
            var notification = new
            {
                Type = "CommentAdded",
                BoardId = boardId,
                Message = $"{commentedBy} commented on '{taskTitle}'",
                Title = taskTitle,
                CommentedBy = commentedBy,
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"board_{boardId}")
                .SendAsync("ReceiveNotification", notification);
        }

        public async Task NotifyColumnCreatedAsync(Guid boardId, string columnName, string createdBy)
        {
            await _hubContext.Clients.Group($"board_{boardId}")
                .SendAsync("RefreshBoard", new { BoardId = boardId, Action = "ColumnCreated" });
        }

        public async Task NotifyColumnUpdatedAsync(Guid boardId, string columnName, string updatedBy)
        {
            await _hubContext.Clients.Group($"board_{boardId}")
                .SendAsync("RefreshBoard", new { BoardId = boardId, Action = "ColumnUpdated" });
        }

        public async Task NotifyColumnDeletedAsync(Guid boardId, string columnName, string deletedBy)
        {

            await _hubContext.Clients.Group($"board_{boardId}")
                .SendAsync("RefreshBoard", new { BoardId = boardId, Action = "ColumnDeleted" });
        }
    }
}