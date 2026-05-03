namespace TaskNest.API.DTOs
{
    public class BoardDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public List<BoardColumnDto> BoardColumns { get; set; } = new();
    }
}