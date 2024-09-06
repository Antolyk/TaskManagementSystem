using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data.Models;
using TaskManagementSystem.Data.Repositories.Contract;

namespace TaskManagementSystem.Data.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public TaskItem GetById(string taskId)
        {
            return _context.Tasks.FirstOrDefault(i => i.Id.ToString() == taskId);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksForUserAsync(string userId)
        {
            return await _context.Tasks
                .Where(t => t.UserId.ToString() == userId)
                .ToListAsync();
        }

        public async Task AddAsync(TaskItem task)
        {
            await _context.Tasks.AddAsync(task);
        }

        public async Task UpdateAsync(TaskItem task)
        {
            var existingTask = await _context.Tasks.FirstOrDefaultAsync(i => i.Id == task.Id);
            if (task != null)
                _context.Tasks.Update(task);
        }

        public async Task DeleteAsync(string taskId)
        {
            var existingTask = await _context.Tasks.FirstOrDefaultAsync(i => i.Id.ToString() == taskId);
            if (existingTask != null)
                _context.Tasks.Remove(existingTask);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
