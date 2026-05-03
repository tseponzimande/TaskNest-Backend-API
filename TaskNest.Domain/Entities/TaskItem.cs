namespace TaskNest.Domain.Entities
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public int Position { get; set; }

        public Guid BoardId { get; set; }
        public Board? Board { get; set; }

        public Guid? ColumnId { get; set; }
        public BoardColumn? Column { get; set; }

    }
}