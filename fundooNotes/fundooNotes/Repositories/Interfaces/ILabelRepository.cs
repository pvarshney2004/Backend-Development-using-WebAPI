using fundooNotes.Models;

namespace fundooNotes.Repositories.Interfaces
{
    public interface ILabelRepository
    {
        Label CreateLabel(Label label);
        bool AddLabelToNote(int noteId, int labelId);
        bool RemoveLabelFromNote(int noteId, int labelId);
        List<Label> GetLabelsByNote(int noteId);
        List<Note> GetNotesByLabel(int labelId);

        bool IsNoteOwnedByUser(int noteId, int userId);
        bool IsUserCollaborator(int noteId, int userId);
        bool IsLabelOwnedByUser(int labelId, int userId);
    }
}
