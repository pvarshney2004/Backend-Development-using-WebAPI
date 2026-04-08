using fundooNotes.DTOs.Notes;
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
    public class NotesController : ControllerBase
    {
        private readonly INotesService _notesService;

        public NotesController(INotesService notesService)
        {
            _notesService = notesService;
        }
        // action methods
        [HttpPost] // used in frontend
        public async Task<IActionResult> CreateNote(CreateNoteDTO dto)
        {
            int userId = GetUserId();
            var note = await _notesService.CreateNote(dto, userId);
            return Ok(note);
        }

        [HttpGet] // used in frontend
        public async Task<IActionResult> GetAllNotes()
        {
            int userId = GetUserId();

            var notes = await _notesService.GetAllNotes(userId);

            return Ok(notes);
        }

        [HttpGet("{noteId}")]
        public async Task<IActionResult> GetNoteById(int noteId)
        {
            int userId = GetUserId();

            var note = await _notesService.GetNoteById(noteId, userId);

            if (note == null)
            {
                return NotFound("Note not found");
            }

            return Ok(note);
        }

        [HttpPut("{noteId}")] // used in frontend
        public async Task<IActionResult> UpdateNote(int noteId, UpdateNoteDTO dto)
        {
            int userId = GetUserId();

            var note = await _notesService.UpdateNote(noteId, dto, userId);

            if (note == null)
            {
                return NotFound("Note not found");
            }

            return Ok(note);
        }


        [HttpPatch("archive/{noteId}")] // used in frontend
        public async Task<IActionResult> ToggleArchive(int noteId)
        {
            int userId = GetUserId();
            var result = await _notesService.ToggleArchive(noteId, userId);
            if (!result)
            {
                return NotFound("Note not found");

            }
            return Ok(new { message = "Note archive status updated" });
        }

        [HttpPatch("pin/{noteId}")] // used in frontend
        public async Task<IActionResult> TogglePin(int noteId)
        {
            int userId = GetUserId();
            var result = await _notesService.TogglePin(noteId, userId);
            if (!result)
            {
                return NotFound("Note not found");

            }

            return Ok(new { message = "pin status updated" });
        }

        [HttpPatch("color/{noteId}")] // used in frontend
        public async Task<IActionResult> ChangeColor(int noteId, ChangeColorDTO dto)
        {
            int userId = GetUserId();

            var result = await _notesService.ChangeColor(noteId, userId, dto.Color);
            if (!result)
            {
                return NotFound("Note not found");

            }

            return Ok(new { message = "Note color updated successfully" });
        }

        [HttpPatch("reminder/{noteId}")] // used in frontend
        public async Task<IActionResult> SetReminder(int noteId, [FromBody] SetReminderDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid request body");
            }
            int userId = GetUserId();
            var result = await _notesService.SetReminder(noteId, userId, dto.Reminder);
            if (!result)
            {
                return NotFound("Note not found");

            }
            return Ok(new { message = "Reminder updated successfully" });
        }

        [HttpGet("trash")] // used in frontend
        public IActionResult GetTrashNotes()
        {
            int userId = GetUserId();

            var notes = _notesService.GetTrashNotes(userId);

            return Ok(notes);
        }
        [HttpGet("archive")] // used in frontend
        public IActionResult GetArchiveNotes()
        {
            int userId = GetUserId();

            var notes = _notesService.GetArchiveNotes(userId);

            return Ok(notes);
        }

        [HttpPatch("restore/{noteId}")] // used in frontend
        public async Task<IActionResult> RestoreNote(int noteId)
        {
            int userId = GetUserId();

            var result = await _notesService.RecoverNote(noteId, userId);

            if (!result)
            {
                return NotFound("Note not found in trash");
            }

            return Ok(new { message = "Note restored successfully" });
        }

        [HttpDelete("{noteId}")] // used in frontend
        public async Task<IActionResult> DeleteNote(int noteId)
        {
            int userId = GetUserId();

            var result = await _notesService.DeleteNote(noteId, userId);

            if (!result)
            {
                return NotFound("Note not found");

            }

            return Ok(new { message = "Note moved to trash" });
        }

        [HttpDelete("permanent/{noteId}")] // used in frontend
        public async Task<IActionResult> DeleteNotePermanent(int noteId)
        {
            int userId = GetUserId();

            var result = await _notesService.DeleteNotePermanently(noteId, userId);
            if (!result)
            {
                return NotFound("Note not found in trash");

            }

            return Ok(new { message = "Note permanently deleted" });
        }



        // GetUserId() gets the UserId stored inside the JWT token that the user sends with the request. 
        private int GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userId);
        }
    }
}

// IActionResult is an interface used in ASP.NET Core controllers to return different types of HTTP responses
// such as OK, BadRequest, NotFound, or Unauthorized to the client.