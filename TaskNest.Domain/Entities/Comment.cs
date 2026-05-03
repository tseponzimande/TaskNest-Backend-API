namespace TaskNest.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Foreign Keys
        public Guid TaskItemId { get; set; }
        public TaskItem? TaskItem { get; set; }

        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }
}