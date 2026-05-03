namespace TaskNest.Domain.Entities
{
    public class BoardColumn
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }

        public Guid BoardId { get; set; }
        public Board? Board { get; set; }

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}