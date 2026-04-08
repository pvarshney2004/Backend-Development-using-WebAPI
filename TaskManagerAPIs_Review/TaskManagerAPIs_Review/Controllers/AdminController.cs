using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerAPIs_Review.Context;

namespace TaskManagerAPIs_Review.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly TaskContext _context;

        public AdminController(TaskContext context)
        {
            _context = context;
        }

        [HttpGet("tasks")]
        public IActionResult GetAllTasks()
        {
            var tasks = _context.Tasks.ToList();
            return Ok(tasks);
        }
    }
}
