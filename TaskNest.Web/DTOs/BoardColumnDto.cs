namespace TaskNest.API.DTOs
{
    public class BoardColumnDto
    {
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}