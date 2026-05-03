namespace TaskNest.API.Services
{
    public class TaskItemService(
        IGenericRepository<TaskItem> repository,
        IEmailService emailService,
        IBoardUserService boardUserService,
        IBoardColumnService boardColumnService,
        INotificationService notificationService) : ITaskItemService
    {
        private readonly IGenericRepository<TaskItem> _repository = repository;
        private readonly IEmailService _emailService = emailService;
        private readonly IBoardUserService _boardUserService = boardUserService;
        private readonly IBoardColumnService _boardColumnService = boardColumnService;
        private readonly INotificationService _notificationService = notificationService;

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            try
            {
                return _repository is GenericRepository<TaskItem> repo
                    ? await repo.Query().Include(t => t.Board).ToListAsync()
                    : await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error fetching all tasks.", ex);
            }
        }

        public async Task<TaskItem?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error fetching task with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<TaskItem>> GetByColumnAsync(Guid columnId)
        {
            try
            {
                return _repository is GenericRepository<TaskItem> repo
                    ? await repo.Query()
                        .Where(t => t.ColumnId == columnId)
                        .OrderBy(t => t.Position)
                        .ToListAsync()
                    : Enumerable.Empty<TaskItem>();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error fetching tasks for column {columnId}.", ex);
            }
        }

        public async Task<TaskItem> CreateAsync(TaskItem taskItem, string userId)
        {
            try
            {
                var isMember = await _boardUserService.IsUserInBoardAsync(taskItem.BoardId, userId);

                if (!isMember)
                {
                    throw new UnauthorizedAccessException("User is not a member of this board.");
                }

                if (_repository is GenericRepository<TaskItem> repo)
                {
                    taskItem.Id = Guid.NewGuid();
                    taskItem.CreatedAt = DateTime.UtcNow;

                    var maxPosition = await repo.Query()
                        .Where(t => t.ColumnId == taskItem.ColumnId)
                        .MaxAsync(t => (int?)t.Position) ?? -1;

                    taskItem.Position = maxPosition + 1;

                    var created = await _repository.CreateAsync(taskItem);

                    var board = await repo.Context.Boards.Include(b => b.User)
                        .FirstOrDefaultAsync(b => b.Id == taskItem.BoardId);

                    if (board?.User?.Email is string email)
                    {
                        await _emailService.SendEmailAsync(
                            email,
                            $"New Task Created: {taskItem.Title}",
                            $"A new task '{taskItem.Title}' was created in your board '{board.Name}'."
                        );
                    }

                    return created;
                }

                return await _repository.CreateAsync(taskItem);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error creating task.", ex);
            }
        }

        public async Task<bool> UpdateAsync(TaskItem taskItem, string userId)
        {
            try
            {
                if (!(_repository is GenericRepository<TaskItem> repo))
                {
                    return await _repository.UpdateAsync(taskItem);
                }

                var existing = await repo.Context.Set<TaskItem>().FindAsync(taskItem.Id);

                if (existing == null)
                {
                    return false;
                }

                var board = await repo.Context.Boards.FindAsync(taskItem.BoardId);

                if (board?.ApplicationUserId != userId)
                {
                    throw new UnauthorizedAccessException("User does not own the board.");
                }

                repo.Context.Entry(existing).CurrentValues.SetValues(taskItem);
                return await repo.Context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error updating task {taskItem.Id}.", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid taskId, string userId)
        {
            try
            {
                var task = await _repository.GetByIdAsync(taskId);

                if (task == null)
                {
                    return false;
                }

                if (_repository is GenericRepository<TaskItem> repo)
                {
                    var board = await repo.Context.Boards.FindAsync(task.BoardId);
                    if (board?.ApplicationUserId != userId)
                    {
                        throw new UnauthorizedAccessException("User does not own the board.");
                    }
                }

                return await _repository.DeleteAsync(taskId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error deleting task {taskId}.", ex);
            }
        }

        public async Task<bool> MoveTaskAsync(Guid taskId, Guid newColumnId, int newPosition, string userId)
        {
            try
            {
                var task = await _repository.GetByIdAsync(taskId);

                if (task == null)
                {
                    return false;
                }

                var isMember = await _boardUserService.IsUserInBoardAsync(task.BoardId, userId);
                if (!isMember)
                {
                    throw new UnauthorizedAccessException("User is not a member of this board.");
                }

                if (_repository is GenericRepository<TaskItem> repo)
                {
                    var context = repo.Context;

                    var fromColumn = task.ColumnId.HasValue
                        ? (await _boardColumnService.GetByIdAsync(task.ColumnId.Value))?.Name ?? "Unknown"
                        : "Unknown";

                    var toColumn = (await _boardColumnService.GetByIdAsync(newColumnId))?.Name ?? "Unknown";

                    task.ColumnId = newColumnId;
                    task.Position = newPosition;

                    var updated = await context.SaveChangesAsync() > 0;

                    if (updated)
                    {
                        await _notificationService.NotifyTaskMovedAsync(
                            task.BoardId,
                            task.Title,
                            userId,
                            fromColumn,
                            toColumn
                        );
                    }

                    return updated;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error moving task {taskId}.", ex);
            }
        }
    }
}
