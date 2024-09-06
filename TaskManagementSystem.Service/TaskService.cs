using Microsoft.Extensions.Logging;
using TaskManagementSystem.Data.Models;
using TaskManagementSystem.Data.Repositories.Contract;
using TaskManagementSystem.Service.Interfaces;
using TaskManagementSystem.Service.Models;

namespace TaskManagementSystem.Service
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserService _userService;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ITaskRepository taskRepository, IUserService userService, ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository;
            _userService = userService;
            _logger = logger;
        }


        public TaskServiceModel GetTaskById(string taskId, string userId)
        {
            _logger.LogInformation("Returning task {taskId} for user: {UserId}", taskId, userId);

            try
            {
                TaskItem task = _taskRepository.GetById(taskId);

                // Check whether the task belongs to the user
                if (task == null || userId.ToLower() != task.UserId.ToString())
                    return null;

                // Create a response model
                TaskServiceModel taskmodel = new TaskServiceModel
                {
                    Id = task.Id.ToString(),
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Status = task.Status,
                    Priority = task.Priority,
                    UserId = task.UserId,
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt
                };

                _logger.LogInformation("Task {taskId} was succesfully returned to user: {UserId}", taskId, userId);
                return taskmodel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while returning task {taskId} for user {UserId}.", taskId, userId);
                throw;
            }
        }

        public IEnumerable<TaskServiceModel> GetTasks(
            int? status, DateTime? dueDate, int? priority,
            string sortField, string sortOrder,
            int pageNumber, int pageSize, string userId)
        {
            _logger.LogInformation("Returning all tasks for user: {UserId}", userId);

            try
            {
                var query = _taskRepository.GetTasksForUserAsync(userId).Result.AsQueryable();

                // Filter by Status
                if (status.HasValue)
                    query = query.Where(t => (int)t.Status == status.Value);

                // Filter by DueDate
                if (dueDate.HasValue)
                    query = query.Where(t => t.DueDate == dueDate.Value);

                // Filter by Priority
                if (priority.HasValue)
                    query = query.Where(t => (int)t.Priority == priority.Value);

                _logger.LogInformation("Tasks was succesfully filtered for user: {UserId}", userId);

                // Sort
                switch (sortField.ToLower())
                {
                    case "duedate":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(t => t.DueDate) : query.OrderByDescending(t => t.DueDate);
                        break;
                    case "priority":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(t => t.Priority) : query.OrderByDescending(t => t.Priority);
                        break;
                    default:
                        query = query.OrderBy(t => t.DueDate); // Sort by DueDate by default
                        break;
                }

                _logger.LogInformation("Tasks was succesfully sorted for user: {UserId}", userId);

                // Pagination
                List<TaskItem> tasks = query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Tasks was succesfully paginated for user: {UserId}", userId);
                _logger.LogInformation("Tasks was succesfully returned for user: {UserId}", userId);

                // Create a response model
                return tasks.Select(t => new TaskServiceModel
                {
                    Id = t.Id.ToString(),
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    Status = t.Status,
                    Priority = t.Priority,
                    UserId = t.UserId,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while returning tasks for user {UserId}.", userId);
                throw;
            }
        }

        public async Task<TaskServiceModel> CreateTaskAsync(TaskDto request, string userId)
        {
            _logger.LogInformation("Creating task for user: {UserId}", userId);

            try
            {
                // Create an entity
                TaskItem task = new TaskItem
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Description = request.Description,
                    DueDate = request.DueDate,
                    Status = request.Status,
                    Priority = request.Priority,
                    UserId = request.UserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _taskRepository.AddAsync(task);
                await _taskRepository.SaveChangesAsync();

                _logger.LogInformation("Task {TaskId} created successfully for user {UserId}.", task.Id, userId);

                // Create a response model
                return new TaskServiceModel
                {
                    Id = task.Id.ToString(),
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Status = task.Status,
                    Priority = task.Priority,
                    UserId = task.UserId,
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating task for user {UserId}.", userId);
                throw;
            }
        }


        public async Task<TaskServiceModel> UpdateTaskAsync(string taskId, string userId, TaskDto request)
        {
            _logger.LogInformation("Updating task {TaskId} for user {UserId}", taskId, userId);

            try
            {
                TaskItem task = _taskRepository.GetById(taskId);

                if (task != null && userId.ToLower() == task.UserId.ToString())
                {
                    // Update task with data that has value in request
                    if (request.Title != null) task.Title = request.Title;
                    if (request.Description != null) task.Description = request.Description;
                    if (request.DueDate != null) task.DueDate = request.DueDate;
                    if (request.Status != null) task.Status = request.Status;
                    if (request.Priority != null) task.Priority = request.Priority;

                    task.UpdatedAt = DateTime.UtcNow;

                    await _taskRepository.UpdateAsync(task);
                    await _taskRepository.SaveChangesAsync();

                    _logger.LogInformation("Task {TaskId} updated successfully for user {UserId}.", taskId, userId);

                    // Create a response model
                    return new TaskServiceModel
                    {
                        Id = task.Id.ToString(),
                        Title = task.Title,
                        Description = task.Description,
                        DueDate = task.DueDate,
                        Status = task.Status,
                        Priority = task.Priority,
                        UserId = task.UserId,
                        CreatedAt = task.CreatedAt,
                        UpdatedAt = task.UpdatedAt
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating task {TaskId} for user {UserId}.", taskId, userId);
                throw;
            }
        }

        public async Task<string> DeleteTaskAsync(string taskId, string userId)
        {
            _logger.LogInformation("Deleting task {TaskId} for user {UserId}.", taskId, userId);

            try
            {
                TaskItem task = _taskRepository.GetById(taskId);

                // Check whether the task belongs to the user
                if (task != null && userId.ToLower() == task.UserId.ToString())
                {
                    await _taskRepository.DeleteAsync(taskId);
                    await _taskRepository.SaveChangesAsync();

                    _logger.LogInformation("Task {TaskId} deleted successfully.", taskId);

                    return "Task was deleted.";
                }

                _logger.LogError("Task {TaskId} was not deleted.", taskId);
                return "Task was not deleted!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting task {TaskId} for user {UserId}.", taskId, userId);
                throw;
            }
        }

    }
}
