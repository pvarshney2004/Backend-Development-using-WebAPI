using fundooNotes.DTOs.Collaborators;
using fundooNotes.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace fundooNotes.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
     public class CollaboratorController : ControllerBase
    {
        private readonly ICollaboratorService _service;

        public CollaboratorController(ICollaboratorService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpPost]
        public IActionResult AddCollaborator(AddCollaboratorDTO dto)
        {
            int userId = GetUserId();

            var result = _service.AddCollaborator(dto, userId);



            return Ok(result);
        }

        [HttpGet("{noteId}")]
        public IActionResult GetCollaborators(int noteId)
        {
            var result = _service.GetCollaborators(noteId);

            return Ok(result);
        }

        [HttpDelete("{collaboratorId}")]
        public IActionResult RemoveCollaborator(int collaboratorId)
        {
            var result = _service.RemoveCollaborator(collaboratorId);

            if (!result)
                return NotFound();

            return Ok("Collaborator removed");
        }
    }
}
