namespace TaskNest.API.DTOs
{
    public class MoveTaskDto
    {
        public Guid ColumnId { get; set; }
        public int Position { get; set; }
    }
}