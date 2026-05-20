namespace Backend.Models
{
    public enum Priority { Low, Medium, High }
    public enum TaskStatus { ToDo, Completed }
    public class TaskModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public Priority Priority { get; set; } = Priority.Medium;
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
