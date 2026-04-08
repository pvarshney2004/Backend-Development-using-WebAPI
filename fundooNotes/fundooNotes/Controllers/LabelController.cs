using fundooNotes.DTOs.Labels;
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
    public class LabelController : ControllerBase
    {
        private readonly ILabelService _service;

        public LabelController(ILabelService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        [HttpPost]
        public IActionResult CreateLabel(CreateLabelDTO dto)
        {
            var result = _service.CreateLabel(dto, GetUserId());
            return Ok(result);
        }

        [HttpPost("add")]
        public IActionResult AddLabel(AddLabelToNoteDTO dto)
        {
            var result = _service.AddLabelToNote(dto, GetUserId());
            return Ok(result);
        }

        [HttpDelete]
        public IActionResult RemoveLabel(int noteId, int labelId)
        {
            var result = _service.RemoveLabel(noteId, labelId, GetUserId());
            return Ok(result);
        }

        [HttpGet("note/{noteId}")]
        public IActionResult GetLabels(int noteId)
        {
            var result = _service.GetLabelsByNote(noteId, GetUserId());
            return Ok(result);
        }

        [HttpGet("{labelId}")]
        public IActionResult GetNotes(int labelId)
        {
            var result = _service.GetNotesByLabel(labelId, GetUserId());
            return Ok(result);
        }
    }
}
