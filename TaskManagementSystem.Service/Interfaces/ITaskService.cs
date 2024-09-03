using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementSystem.Service.Interfaces
{
    public interface ITaskService
    {
        Task<Task> CreateTask(Task task);
        Task<Task> UpdateTask(Task task);
        Task DeleteTask(Guid taskId);
        Task<Task> GetTaskById(Guid taskId);
        Task<IEnumerable<Task>> GetTasksByUserId(Guid userId);
    }
}
