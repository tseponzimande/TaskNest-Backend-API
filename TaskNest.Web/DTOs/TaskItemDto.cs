namespace TaskNest.API.DTOs
{
    public class TaskItemDto
    {
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public Guid? ColumnId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
    }
}