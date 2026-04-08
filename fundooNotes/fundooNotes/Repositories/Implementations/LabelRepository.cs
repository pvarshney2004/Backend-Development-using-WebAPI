using fundooNotes.Context;
using fundooNotes.Models;
using fundooNotes.Repositories.Interfaces;

namespace fundooNotes.Repositories.Implementations
{
    public class LabelRepository : ILabelRepository
    {
        private readonly FundooContext _context;

        public LabelRepository(FundooContext context)
        {
            _context = context;
        }

        public Label CreateLabel(Label label)
        {
            _context.Labels.Add(label);
            _context.SaveChanges();
            return label;
        }

        public bool AddLabelToNote(int noteId, int labelId)
        {
            var exists = _context.NoteLabels
                .Any(nl => nl.NoteId == noteId && nl.LabelId == labelId);

            if (exists) return false;

            _context.NoteLabels.Add(new NoteLabel
            {
                NoteId = noteId,
                LabelId = labelId
            });

            _context.SaveChanges();
            return true;
        }

        public bool RemoveLabelFromNote(int noteId, int labelId)
        {
            var entry = _context.NoteLabels
                .FirstOrDefault(nl => nl.NoteId == noteId && nl.LabelId == labelId);

            if (entry == null) return false;

            _context.NoteLabels.Remove(entry);
            _context.SaveChanges();
            return true;
        }

        public List<Label> GetLabelsByNote(int noteId)
        {
            return _context.NoteLabels
                .Where(nl => nl.NoteId == noteId)
                .Select(nl => nl.Label)
                .ToList();
        }

        public List<Note> GetNotesByLabel(int labelId)
        {
            return _context.NoteLabels
                .Where(nl => nl.LabelId == labelId)
                .Select(nl => nl.Note)
                .ToList();
        }
        public bool IsNoteOwnedByUser(int noteId, int userId)
        {
            return _context.Notes.Any(n => n.NoteId == noteId && n.UserId == userId);
        }

        public bool IsUserCollaborator(int noteId, int userId)
        {
            return _context.Collaborators
                .Any(c => c.NoteId == noteId && c.UserId == userId);
        }

        public bool IsLabelOwnedByUser(int labelId, int userId)
        {
            return _context.Labels
                .Any(l => l.LabelId == labelId && l.UserId == userId);
        }
    }
}
