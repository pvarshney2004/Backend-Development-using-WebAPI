using fundooNotes.DTOs.Labels;
using fundooNotes.Models;
using fundooNotes.Repositories.Interfaces;
using fundooNotes.Services.Interfaces;

namespace fundooNotes.Services.Implementations
{
    public class LabelService : ILabelService
    {
        private readonly ILabelRepository _repo;

        public LabelService(ILabelRepository repo)
        {
            _repo = repo;
        }

        public Label CreateLabel(CreateLabelDTO dto, int userId)
        {
            var label = new Label
            {
                LabelName = dto.LabelName,
                UserId = userId
            };

            return _repo.CreateLabel(label);
        }

        public bool AddLabelToNote(AddLabelToNoteDTO dto, int userId)
        {
            // Check note access
            bool hasAccess = _repo.IsNoteOwnedByUser(dto.NoteId, userId) ||
                             _repo.IsUserCollaborator(dto.NoteId, userId);

            if (!hasAccess)
                throw new Exception("Access denied to this note");

            // Check label ownership
            if (!_repo.IsLabelOwnedByUser(dto.LabelId, userId))
                throw new Exception("Label does not belong to user");

            return _repo.AddLabelToNote(dto.NoteId, dto.LabelId);
        }

        public bool RemoveLabel(int noteId, int labelId, int userId)
        {
            bool hasAccess = _repo.IsNoteOwnedByUser(noteId, userId) ||
                             _repo.IsUserCollaborator(noteId, userId);

            if (!hasAccess)
                throw new Exception("Access denied");

            return _repo.RemoveLabelFromNote(noteId, labelId);
        }

        public List<Label> GetLabelsByNote(int noteId, int userId)
        {
            bool hasAccess = _repo.IsNoteOwnedByUser(noteId, userId) ||
                             _repo.IsUserCollaborator(noteId, userId);

            if (!hasAccess)
                throw new Exception("Access denied");

            return _repo.GetLabelsByNote(noteId);
        }

        public List<Note> GetNotesByLabel(int labelId, int userId)
        {
            // Label must belong to user
            if (!_repo.IsLabelOwnedByUser(labelId, userId))
                throw new Exception("Invalid label");

            return _repo.GetNotesByLabel(labelId)
                .Where(n => n.UserId == userId ||
                            _repo.IsUserCollaborator(n.NoteId, userId))
                .ToList();
        }
    }
}
