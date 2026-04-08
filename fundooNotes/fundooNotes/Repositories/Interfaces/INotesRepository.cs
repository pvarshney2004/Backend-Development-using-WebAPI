using fundooNotes.Models;

namespace fundooNotes.Repositories.Interfaces
{
    public interface INotesRepository
    {
        // CRUD operations for Note entity
        Note CreateNote(Note note);

        List<Note> GetAllNotes(int userId);

        Note GetNoteById(int noteId, int userId);

        Note UpdateNote(Note note, int userId);

        void DeleteNote(Note note);
        bool ToggleArchive(int noteId, int userId);


        bool SetReminder(int noteId, int userId, DateTime? reminder);

        List<Note> GetTrashNotes(int userId);

        List<Note> GetArchiveNotes(int userId);
        bool TogglePin(int noteId, int userId);
        bool ChangeColor(int noteId, int userId, string color);

        bool RecoverNote(int noteId, int userId);

        bool DeleteNotePermanently(int noteId, int userId);
    }
}
