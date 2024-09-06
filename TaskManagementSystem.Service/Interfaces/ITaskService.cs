using TaskManagementSystem.Data.Models;
using TaskManagementSystem.Service.Models;


namespace TaskManagementSystem.Service.Interfaces
{
    public interface ITaskService
    {
        TaskServiceModel GetTaskById(string taskId, string userId);
        IEnumerable<TaskServiceModel> GetTasks(
            int? status, DateTime? dueDate, int? priority,
            string sortField, string sortOrder,
            int pageNumber, int pageSize, string userId);
        Task<TaskServiceModel> CreateTaskAsync(TaskDto request, string userId);
        Task<TaskServiceModel> UpdateTaskAsync(string taskId, string userId, TaskDto request);
        Task<string> DeleteTaskAsync(string taskId, string userId);
    }
}
