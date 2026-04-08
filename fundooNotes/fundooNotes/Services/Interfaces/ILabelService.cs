using fundooNotes.DTOs.Labels;
using fundooNotes.Models;

namespace fundooNotes.Services.Interfaces
{
    public interface ILabelService
    {
        Label CreateLabel(CreateLabelDTO dto, int userId);
        bool AddLabelToNote(AddLabelToNoteDTO dto, int userId);
        bool RemoveLabel(int noteId, int labelId, int userId);
        List<Label> GetLabelsByNote(int noteId, int userId);
        List<Note> GetNotesByLabel(int labelId, int userId);
    }
}
