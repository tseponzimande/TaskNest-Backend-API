namespace TaskNest.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsEnabled { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public ICollection<Board>? Boards { get; set; } = new List<Board>();

        // Navigation
        public ICollection<BoardUser> BoardUsers { get; set; } = new List<BoardUser>();
    }
}