namespace TaskNest.Domain.Entities
{
    public class TaskAttachment
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public Guid TaskItemId { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;

        // Navigation
        public TaskItem? TaskItem { get; set; }
        public ApplicationUser? User { get; set; }
    }
}