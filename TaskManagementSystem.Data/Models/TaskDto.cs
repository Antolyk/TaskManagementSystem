namespace TaskManagementSystem.Data.Models
{
    public class TaskDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }

        public Guid UserId { get; set; }
    }
}
