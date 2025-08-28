namespace ToDo.Models
{

    public enum TaskStatus
    {
        Todo,
        InProgress,
        Done
    }
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.Todo;




        // Foreign Key
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}