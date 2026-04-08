using fundooNotes.DTOs.Notes;

namespace fundooNotes.Services.Interfaces
{
    public interface INotesService
    {
        Task<NoteResponseDTO> CreateNote(CreateNoteDTO dto, int userId); //cached
        Task<List<NoteResponseDTO>> GetAllNotes(int userId); //cached
        Task<NoteResponseDTO> GetNoteById(int noteId, int userId); 

        Task<NoteResponseDTO> UpdateNote(int noteId, UpdateNoteDTO dto, int userId); //cached

        Task<bool> DeleteNote(int noteId, int userId);
        Task<bool> ToggleArchive(int noteId, int userId);
        Task<bool> TogglePin(int noteId, int userId);

        Task<bool> ChangeColor(int noteId, int userId, string color);

        Task<bool> SetReminder(int noteId, int userId, DateTime? reminder);

        List<NoteResponseDTO> GetTrashNotes(int userId);
        List<NoteResponseDTO> GetArchiveNotes(int userId);

        Task<bool> RecoverNote(int noteId, int userId);

        Task<bool> DeleteNotePermanently(int noteId, int userId);
    }
}
