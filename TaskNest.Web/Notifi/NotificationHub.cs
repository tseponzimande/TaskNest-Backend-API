namespace TaskNest.API.Notifi
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
                Console.WriteLine($"User {userId} connected to notification hub");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
                    Console.WriteLine($"User {userId} disconnected from notification hub");
                }
                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task JoinBoardGroup(string boardId)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"board_{boardId}");
                Console.WriteLine($"Client joined board group: {boardId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task LeaveBoardGroup(string boardId)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"board_{boardId}");
                Console.WriteLine($"Client left board group: {boardId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task NotifyTaskDragStart(string boardId, string taskId, string userId)
        {
            try
            {
                await Clients.GroupExcept($"board_{boardId}", Context.ConnectionId)
                    .SendAsync("TaskDragStarted", new { TaskId = taskId, UserId = userId });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task NotifyTaskDragEnd(string boardId, string taskId)
        {
            try
            {
                await Clients.GroupExcept($"board_{boardId}", Context.ConnectionId)
                    .SendAsync("TaskDragEnded", new { TaskId = taskId });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task NotifyColumnReordered(string boardId, object columnOrders)
        {
            try
            {
                await Clients.GroupExcept($"board_{boardId}", Context.ConnectionId)
                    .SendAsync("ColumnReordered", columnOrders);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}