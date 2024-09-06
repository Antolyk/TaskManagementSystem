using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Data.Models;
using TaskManagementSystem.Service.Interfaces;
using TaskManagementSystem.Service.Models;

namespace TaskManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  //Authorization for all endpoints in controller
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost("tasks")]
        public async Task<ActionResult<TaskServiceModel>> CreateTask(TaskDto request, string userId)
        {
            var task = await _taskService.CreateTaskAsync(request, userId);
            if (task == null)
                return BadRequest("You can not add tasks to other users");

            return Ok(task);
        }

        [HttpGet("tasks/{id}")]
        public ActionResult<TaskServiceModel> GetTaskById(string id, string userId)
        {
            var task = _taskService.GetTaskById(id, userId);
            if (task == null)
                return BadRequest("There is an error! Something went wrong.");

            return task;
        }

        [HttpGet("tasks")]
        public ActionResult<IEnumerable<TaskServiceModel>> GetAllTasks(
            string userId, //User
            int? status = null, DateTime? dueDate = null, int? priority = null, //Filter
            string sortField = "DueDate", string sortOrder = "ASC", //Sort
            int pageNumber = 1, int pageSize = 10) //Pagination
        {
            var tasks = _taskService.GetTasks(status, dueDate, priority, sortField, sortOrder, pageNumber, pageSize, userId);
            if (tasks == null)
                return BadRequest("There is no tasks");

            return Ok(tasks);
        }

        [HttpPut("tasks/{id}")]
        public async Task<ActionResult<TaskServiceModel>> UpdateTask(string id, string userId, TaskDto request)
        {
            var task = await _taskService.UpdateTaskAsync(id, userId, request);
            if (task == null)
                return BadRequest("There is an error! Something went wrong.");
            return task;
        }

        [HttpDelete("tasks/{id}")]
        public async Task<ActionResult<string>> DeleteTaskById(string id, string userId)
        {
            return await _taskService.DeleteTaskAsync(id, userId);
        }
    }
}
