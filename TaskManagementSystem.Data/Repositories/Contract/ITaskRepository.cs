using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementSystem.Data.Models;

namespace TaskManagementSystem.Data.Repositories.Contract
{
    public interface ITaskRepository
    {
        Task<TaskItem> GetByIdAsync(Guid id);
        Task<IEnumerable<TaskItem>> GetTasksForUserAsync(Guid userId, int pageIndex, int pageSize);
        Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(TaskItem task);
        Task SaveChangesAsync();
    }
}
