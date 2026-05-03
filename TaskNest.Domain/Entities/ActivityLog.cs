namespace TaskNest.Domain.Entities
{
    public class ActivityLog
    {
        public Guid Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public Guid TaskItemId { get; set; }
        public TaskItem? TaskItem { get; set; }

        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }
}