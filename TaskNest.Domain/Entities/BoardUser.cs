namespace TaskNest.Domain.Entities
{
    public class BoardUser
    {
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public Board Board { get; set; } = null!;
        public string ApplicationUserId { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public BoardUserRole Role { get; set; }
    }

    public enum BoardUserRole
    {
        Viewer,
        Editor,
        Admin
    }
}