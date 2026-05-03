namespace TaskNest.API.DTOs
{
    public class BoardUserDto
    {
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public string ApplicationUserId { get; set; } = null!;
        public BoardUserRole Role { get; set; }
        public string? UserEmail { get; set; }
    }
}