using TaskManagementSystem.Data.Models;
using TaskStatus = TaskManagementSystem.Data.Models.TaskStatus;

namespace TaskManagementSystem.Service.Models
{
    public class TaskServiceModel
    {
        public string Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskStatus? Status { get; set; }
        public TaskPriority? Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Guid UserId { get; set; }
    }
}
