using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerAPIs_Review.DTOs.Task;
using TaskManagerAPIs_Review.Services.Interface;

namespace TaskManagerAPIs_Review.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskDTO createTaskDTO)
        {
            int userId = GetUserId();
            var task = await _taskService.CreateTask(createTaskDTO, userId);
            return Ok(task);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            int userId = GetUserId();
            var tasks = await _taskService.GetAllTasks(userId);
            return Ok(tasks);
        }


        [HttpGet("{taskId}")]
        public IActionResult GetTaskById(int taskId)
        {
            int userId = GetUserId();
            var task = _taskService.GetTaskById(taskId, userId);
            return Ok(task);
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(UpdateTaskDTO updateTaskDTO, int taskId)
        {
            int userId = GetUserId();
            var task =  await _taskService.UpdateTask(taskId, updateTaskDTO, userId);
            return Ok(task);
        }

        [HttpPatch("{taskId}/complete")]
        public IActionResult MarkCompleted(int taskId)
        {
            int userId = GetUserId();
            var ans =  _taskService.MarkComplete(taskId, userId);
            if (ans)
            {
                return Ok(new { message = "task status updated" });
            }
            else
            {
                return BadRequest("updation failed");
            }
        }
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            int userId = GetUserId();
            var ans =  await _taskService.DeleteTask(taskId, userId);
            if (ans)
            {
                return Ok(new { message = "task deleted sucessfully" });
            }
            else
            {
                return BadRequest("Deletion failed");
            }
        }

        private int GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userId);
        }


    }
}
