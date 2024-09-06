using TaskManagementSystem.Data.Models;

namespace TaskManagementSystem.Data.Repositories.Contract
{
    public interface ITaskRepository
    {
        TaskItem GetById(string id);
        Task<IEnumerable<TaskItem>> GetTasksForUserAsync(string userId);
        Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(string taskId);
        Task SaveChangesAsync();
    }
}
