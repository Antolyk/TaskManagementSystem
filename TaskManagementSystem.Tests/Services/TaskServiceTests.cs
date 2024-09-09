using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Data.Models;
using TaskManagementSystem.Data.Repositories.Contract;
using TaskManagementSystem.Service;
using TaskManagementSystem.Service.Interfaces;
using TaskStatus = TaskManagementSystem.Data.Models.TaskStatus;

namespace TaskManagementSystem.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly TaskService _taskService;
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<ILogger<TaskService>> _loggerMock;

        public TaskServiceTests()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<TaskService>>();
            _taskService = new TaskService(_taskRepositoryMock.Object, _userServiceMock.Object, _loggerMock.Object);
        }

        #region CreateTaskAsync
        [Fact]
        public async Task CreateTaskAsync_ShouldCreateTaskSuccessfully()
        {
            // Arrange
            var taskDto = new TaskDto
            {
                Title = "New Task",
                Description = "Task description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = TaskPriority.Low,
                Status = TaskStatus.Pending,
                UserId = Guid.NewGuid()
            };
            _userServiceMock.Setup(x => x.CheckUserById(It.IsAny<string>())).Returns(true);

            // Act
            var result = await _taskService.CreateTaskAsync(taskDto, taskDto.UserId.ToString());

            // Assert
            _taskRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TaskItem>()), Times.Once);
            Assert.Equal(taskDto.Title, result.Title);
            Assert.Equal(taskDto.Description, result.Description);
        }

        [Fact]
        public async Task CreateTaskAsync_ShouldFail_WhenNoTitle()
        {
            // Arrange
            var taskDto = new TaskDto
            {
                Title = null,
                Description = "Task description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = TaskPriority.Low,
                Status = TaskStatus.Pending,
                UserId = Guid.NewGuid()
            };

            // Act
            var result = await _taskService.CreateTaskAsync(taskDto, taskDto.UserId.ToString());

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region GetTaskById
        [Fact]
        public void GetTaskById_ShouldReturnTaskSuccessfully()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();
            var taskItem = new TaskItem
            {
                Id = Guid.Parse(taskId),
                Title = "Sample Task",
                Description = "Task description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = TaskPriority.Low,
                Status = TaskStatus.Pending,
                UserId = Guid.Parse(userId),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _taskRepositoryMock.Setup(x => x.GetById(taskId)).Returns(taskItem);

            // Act
            var result = _taskService.GetTaskById(taskId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskId, result.Id);
            Assert.Equal("Sample Task", result.Title);
        }

        [Fact]
        public void GetTaskById_ShouldReturnNull_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();

            _taskRepositoryMock.Setup(x => x.GetById(taskId)).Returns((TaskItem)null);

            // Act
            var result = _taskService.GetTaskById(taskId, userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetTaskById_ShouldReturnNull_WhenUserDoesNotMatch()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            var validUserId = Guid.NewGuid().ToString();
            var invalidUserId = Guid.NewGuid().ToString();
            var taskItem = new TaskItem
            {
                Id = Guid.Parse(taskId),
                Title = "Sample Task",
                Description = "Task description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = TaskPriority.Low,
                Status = TaskStatus.Pending,
                UserId = Guid.Parse(validUserId),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _taskRepositoryMock.Setup(x => x.GetById(taskId)).Returns(taskItem);

            // Act
            var result = _taskService.GetTaskById(taskId, invalidUserId);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region GetTasks
        [Fact]
        public void GetTasks_ShouldReturnFilteredTasksSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var tasks = new List<TaskItem>
            {
                new TaskItem
                {
                    Id = Guid.NewGuid(),
                    Title = "Task 1",
                    Description = "Description 1",
                    DueDate = DateTime.UtcNow.AddDays(1),
                    Priority = TaskPriority.Low,
                    Status = TaskStatus.Pending,
                    UserId = Guid.Parse(userId),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TaskItem
                {
                    Id = Guid.NewGuid(),
                    Title = "Task 2",
                    Description = "Description 2",
                    DueDate = DateTime.UtcNow.AddDays(2),
                    Priority = TaskPriority.High,
                    Status = TaskStatus.Completed,
                    UserId = Guid.Parse(userId),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _taskRepositoryMock.Setup(x => x.GetTasksForUserAsync(userId)).ReturnsAsync(tasks);

            // Act
            var result = _taskService.GetTasks(null, null, null, "duedate", "asc", 1, 10, userId);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Task 1", result.First().Title);
        }

        [Fact]
        public void GetTasks_ShouldHandleEmptyResults()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            _taskRepositoryMock.Setup(x => x.GetTasksForUserAsync(userId)).ReturnsAsync(new List<TaskItem>());

            // Act
            var result = _taskService.GetTasks(null, null, null, "duedate", "asc", 1, 10, userId);

            // Assert
            Assert.Empty(result);
        }
        #endregion

        #region UpdateTaskAsync
        [Fact]
        public async Task UpdateTaskAsync_ShouldUpdateTaskSuccessfully()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();
            var taskItem = new TaskItem
            {
                Id = Guid.Parse(taskId),
                Title = "Old Task Title",
                Description = "Old description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = TaskPriority.Low,
                Status = TaskStatus.Pending,
                UserId = Guid.Parse(userId),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var updatedTaskDto = new TaskDto
            {
                Title = "Updated Task Title",
                Description = "Updated description",
                DueDate = DateTime.UtcNow.AddDays(2),
                Priority = TaskPriority.High,
                Status = TaskStatus.Completed,
                UserId = Guid.Parse(userId)
            };

            _taskRepositoryMock.Setup(x => x.GetById(taskId)).Returns(taskItem);

            // Act
            var result = await _taskService.UpdateTaskAsync(taskId, userId, updatedTaskDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Task Title", result.Title);
            Assert.Equal("Updated description", result.Description);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldReturnNull_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();

            _taskRepositoryMock.Setup(x => x.GetById(taskId)).Returns((TaskItem)null);

            // Act
            var result = await _taskService.UpdateTaskAsync(taskId, userId, new TaskDto());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldReturnNull_WhenUserDoesNotMatch()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            var validUserId = Guid.NewGuid().ToString();
            var invalidUserId = Guid.NewGuid().ToString();
            var taskItem = new TaskItem
            {
                Id = Guid.Parse(taskId),
                Title = "Sample Task",
                Description = "Task description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = TaskPriority.Low,
                Status = TaskStatus.Pending,
                UserId = Guid.Parse(validUserId),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _taskRepositoryMock.Setup(x => x.GetById(taskId)).Returns(taskItem);

            // Act
            var result = await _taskService.UpdateTaskAsync(taskId, invalidUserId, new TaskDto());

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region DeleteTaskAsync
        [Fact]
        public async Task DeleteTaskAsync_ShouldDeleteTaskSuccessfully()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();
            var taskItem = new TaskItem
            {
                Id = Guid.Parse(taskId),
                Title = "Task to delete",
                Description = "Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = TaskPriority.Low,
                Status = TaskStatus.Pending,
                UserId = Guid.Parse(userId),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _taskRepositoryMock.Setup(x => x.GetById(taskId)).Returns(taskItem);

            // Act
            var result = await _taskService.DeleteTaskAsync(taskId, userId);

            // Assert
            _taskRepositoryMock.Verify(x => x.DeleteAsync(taskId), Times.Once);
            Assert.Equal("Task was deleted.", result);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldReturnError_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();

            _taskRepositoryMock.Setup(x => x.GetById(taskId)).Returns((TaskItem)null);

            // Act
            var result = await _taskService.DeleteTaskAsync(taskId, userId);

            // Assert
            Assert.Equal("Task was not deleted!", result);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldReturnError_WhenUserDoesNotMatch()
        {
            // Arrange
            var taskId = Guid.NewGuid().ToString();
            var validUserId = Guid.NewGuid().ToString();
            var invalidUserId = Guid.NewGuid().ToString();
            var taskItem = new TaskItem
            {
                Id = Guid.Parse(taskId),
                Title = "Task to delete",
                Description = "Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = TaskPriority.Low,
                Status = TaskStatus.Pending,
                UserId = Guid.Parse(validUserId),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _taskRepositoryMock.Setup(x => x.GetById(taskId)).Returns(taskItem);

            // Act
            var result = await _taskService.DeleteTaskAsync(taskId, invalidUserId);

            // Assert
            Assert.Equal("Task was not deleted!", result);
        }
        #endregion
    }
}