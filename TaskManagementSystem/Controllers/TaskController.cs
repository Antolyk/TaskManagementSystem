using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        public async Task<ActionResult<TaskServiceModel>> CreateTask(TaskDto request)
        {
            var userId = User.FindFirst("Identifier")?.Value;
            if (userId == null)
                return Unauthorized();

            var task = await _taskService.CreateTaskAsync(request, userId);
            if (task == null)
                return BadRequest("You can not add tasks to other users");

            return Ok(task);
        }

        [HttpGet("tasks/{id}")]
        public ActionResult<TaskServiceModel> GetTaskById(string id)
        {
            var userId = User.FindFirst("Identifier")?.Value;
            if (userId == null)
                return Unauthorized();

            var task = _taskService.GetTaskById(id, userId);
            if (task == null)
                return NotFound();

            return task;
        }

        [HttpGet("tasks")]
        public ActionResult<IEnumerable<TaskServiceModel>> GetAllTasks(
            int? status = null, DateTime? dueDate = null, int? priority = null, //Filter
            string sortField = "DueDate", string sortOrder = "ASC", //Sort
            int pageNumber = 1, int pageSize = 10) //Pagination
        {
            var userId = User.FindFirst("Identifier")?.Value;
            if (userId == null)
                return Unauthorized();

            var tasks = _taskService.GetTasks(status, dueDate, priority, sortField, sortOrder, pageNumber, pageSize, userId);
            if (tasks == null)
                return BadRequest("There is no tasks");

            return Ok(tasks);
        }

        [HttpPut("tasks/{id}")]
        public async Task<ActionResult<TaskServiceModel>> UpdateTask(string id, TaskDto request)
        {
            var userId = User.FindFirst("Identifier")?.Value;
            if (userId == null)
                return Unauthorized();

            var task = await _taskService.UpdateTaskAsync(id, userId, request);
            if (task == null)
                return BadRequest("There is an error! Something went wrong.");
            return task;
        }

        [HttpDelete("tasks/{id}")]
        public async Task<ActionResult<string>> DeleteTaskById(string id)
        {
            var userId = User.FindFirst("Identifier")?.Value;
            if (userId == null)
                return Unauthorized();

            return await _taskService.DeleteTaskAsync(id, userId);
        }
    }
}
