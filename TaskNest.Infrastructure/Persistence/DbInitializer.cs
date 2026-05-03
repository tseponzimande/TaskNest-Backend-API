namespace TaskNest.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!await context.Boards.AnyAsync())
            {
                var board = new Board
                {
                    Id = Guid.NewGuid(),
                    Name = "Project Alpha",
                    Description = "Main project board",
                    Tasks = new List<TaskItem>
                    {
                        new TaskItem
                        {
                            Id = Guid.NewGuid(),
                            Title = "Design UI",
                            Description = "Create initial mockups",
                            CreatedAt = DateTime.UtcNow,
                            DueDate = DateTime.UtcNow.AddDays(7),
                        },
                        new TaskItem
                        {
                            Id = Guid.NewGuid(),
                            Title = "Set up CI/CD",
                            Description = "Add GitHub Actions workflow",
                            CreatedAt = DateTime.UtcNow,
                            DueDate = DateTime.UtcNow.AddDays(5),
                        }
                    }
                };
                await context.Boards.AddAsync(board);
                await context.SaveChangesAsync();

                var columns = new List<BoardColumn>
                {
                    new BoardColumn 
                    {
                        Id = Guid.NewGuid(),
                        Name = "To Do", 
                        Order = 1, 
                        BoardId = board.Id 
                    },

                    new BoardColumn 
                    {
                        Id = Guid.NewGuid(),
                        Name = "In Progress",
                        Order = 2,
                        BoardId = board.Id
                    },

                    new BoardColumn 
                    { 
                        Id = Guid.NewGuid(),
                        Name = "Done",
                        Order = 3,
                        BoardId = board.Id
                    }
                };
                await context.BoardColumns.AddRangeAsync(columns);
                await context.SaveChangesAsync();
            }
            else if (!await context.BoardColumns.AnyAsync())
            {
                var existingBoard = await context.Boards.FirstAsync();
                var columns = new List<BoardColumn>
                {
                    new BoardColumn 
                    {
                        Id = Guid.NewGuid(),
                        Name = "To Do",
                        Order = 1,
                        BoardId = existingBoard.Id
                    },

                    new BoardColumn
                    {
                        Id = Guid.NewGuid(),
                        Name = "In Progress",
                        Order = 2, 
                        BoardId = existingBoard.Id 
                    },
                    new BoardColumn 
                    { 
                        Id = Guid.NewGuid(),
                        Name = "Done",
                        Order = 3, 
                        BoardId = existingBoard.Id
                    }
                };
                await context.BoardColumns.AddRangeAsync(columns);
                await context.SaveChangesAsync();
            }
        }
    }
}
