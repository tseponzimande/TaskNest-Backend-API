namespace TaskNest.API.DTOs
{
    public class ReorderColumnsDto
    {
        public Guid BoardId { get; set; }
        public List<Guid> OrderedColumnIds { get; set; } = new();
    }
}
