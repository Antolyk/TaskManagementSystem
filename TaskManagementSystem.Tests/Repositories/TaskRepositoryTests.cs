using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data.Models;
using TaskManagementSystem.Data.Repositories;
using TaskStatus = TaskManagementSystem.Data.Models.TaskStatus;

namespace TaskManagementSystem.Tests.Repositories
{
    public class TaskRepositoryTests
    {
        private readonly TaskRepository _taskRepository;
        private readonly ApplicationDbContext _context;

        public TaskRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _taskRepository = new TaskRepository(_context);
        }

        #region AddAsync
        [Fact]
        public async Task AddAsync_ShouldAddTask()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Test Task",
                Description = "Description of Test Task",
                DueDate = DateTime.Now,
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                UserId = Guid.NewGuid()
            };

            // Act
            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            // Assert
            var retrievedTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
            Assert.NotNull(retrievedTask);
            Assert.Equal("Test Task", retrievedTask.Title);
        }

        [Fact]
        public async Task AddAsync_ShouldFailToAddTask()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Test Task",
                Description = "Description of Test Task",
                DueDate = DateTime.Now,
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                UserId = Guid.NewGuid()
            };

            // Act
            await _taskRepository.AddAsync(task);
            // Intentionally skip SaveChangesAsync to simulate failure

            // Assert
            var retrievedTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
            Assert.Null(retrievedTask); // This should fail because SaveChangesAsync was not called
        }
        #endregion

        #region GetById
        [Fact]
        public async Task GetById_ShouldReturnTask()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Test Task",
                Description = "Description of Test Task",
                DueDate = DateTime.Now,
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                UserId = Guid.NewGuid()
            };

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            // Act
            var retrievedTask = _taskRepository.GetById(task.Id.ToString());

            // Assert
            Assert.NotNull(retrievedTask);
            Assert.Equal(task.Id, retrievedTask.Id);
            Assert.Equal("Test Task", retrievedTask.Title);
        }

        [Fact]
        public async Task GetById_ShouldFailToReturnTask()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Test Task",
                Description = "Description of Test Task",
                DueDate = DateTime.Now,
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                UserId = Guid.NewGuid()
            };

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            // Act
            var retrievedTask = _taskRepository.GetById(Guid.NewGuid().ToString()); // Use a different ID

            // Assert
            Assert.Null(retrievedTask); // This should pass because ID does not exist
        }
        #endregion

        #region GetTasksForUserAsync
        [Fact]
        public async Task GetTasksForUserAsync_ShouldReturnTasksForUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var task1 = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "User Task 1",
                Description = "Description of User Task 1",
                DueDate = DateTime.Now,
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                UserId = userId
            };

            var task2 = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "User Task 2",
                Description = "Description of User Task 2",
                DueDate = DateTime.Now,
                Status = TaskStatus.Completed,
                Priority = TaskPriority.High,
                UserId = userId
            };

            await _taskRepository.AddAsync(task1);
            await _taskRepository.AddAsync(task2);
            await _taskRepository.SaveChangesAsync();

            // Act
            var tasks = await _taskRepository.GetTasksForUserAsync(userId.ToString());

            // Assert
            Assert.Equal(2, tasks.Count());
            Assert.Contains(tasks, t => t.Title == "User Task 1");
            Assert.Contains(tasks, t => t.Title == "User Task 2");
        }

        [Fact]
        public async Task GetTasksForUserAsync_ShouldFailToReturnTasksForUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var task1 = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "User Task 1",
                Description = "Description of User Task 1",
                DueDate = DateTime.Now,
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                UserId = userId
            };

            var task2 = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "User Task 2",
                Description = "Description of User Task 2",
                DueDate = DateTime.Now,
                Status = TaskStatus.Completed,
                Priority = TaskPriority.High,
                UserId = userId
            };

            await _taskRepository.AddAsync(task1);
            await _taskRepository.AddAsync(task2);
            await _taskRepository.SaveChangesAsync();

            // Act
            var tasks = await _taskRepository.GetTasksForUserAsync(Guid.NewGuid().ToString()); // Use a different user ID

            // Assert
            Assert.Empty(tasks); // This should pass because user ID does not match
        }
        #endregion

        #region UpdateAsync
        [Fact]
        public async Task UpdateAsync_ShouldUpdateTask()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Old Title",
                Description = "Old Description",
                DueDate = DateTime.Now,
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                UserId = Guid.NewGuid()
            };

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            // Act
            task.Title = "New Title";
            task.Description = "New Description";

            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();

            // Assert
            var updatedTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
            Assert.NotNull(updatedTask);
            Assert.Equal("New Title", updatedTask.Title);
            Assert.Equal("New Description", updatedTask.Description);
        }
        #endregion

        #region DeleteAsync
        [Fact]
        public async Task DeleteAsync_ShouldRemoveTask()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task to Delete",
                Description = "Description of Task to Delete",
                DueDate = DateTime.Now,
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                UserId = Guid.NewGuid()
            };

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            // Act
            await _taskRepository.DeleteAsync(task.Id.ToString());
            await _taskRepository.SaveChangesAsync();

            // Assert
            var deletedTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
            Assert.Null(deletedTask);
        }

        [Fact]
        public async Task DeleteAsync_ShouldFailToRemoveTask()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task to Delete",
                Description = "Description of Task to Delete",
                DueDate = DateTime.Now,
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                UserId = Guid.NewGuid()
            };

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            // Act
            // Simulate a delete failure by not calling DeleteAsync or SaveChangesAsync

            // Assert
            var existingTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
            Assert.NotNull(existingTask); // The task should still exist
        }
        #endregion
    }
}
