using fundooNotes.DTOs.Collaborators;
using fundooNotes.Models;
using fundooNotes.Repositories.Interfaces;
using fundooNotes.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace fundooNotes.Services.Implementations
{
    public class CollaboratorService : ICollaboratorService
    {
        private readonly ICollaboratorRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly INotesRepository _notesRepo;
        private readonly IEmailService _emailService;

        public CollaboratorService(
            ICollaboratorRepository repo,
            IUserRepository userRepo,
            INotesRepository notesRepo,
            IEmailService emailService)
        {
            _repo = repo;
            _userRepo = userRepo;
            _notesRepo = notesRepo;
            _emailService = emailService;
        }

        public Collaborator AddCollaborator(AddCollaboratorDTO dto, int ownerUserId)
        {
            // find collaborator user using email
            var user = _userRepo.GetUserByEmail(dto.Email);

            if (user == null)
                throw new Exception("User not found");

            // check if note belongs to owner
            var note = _notesRepo.GetNoteById(dto.NoteId, ownerUserId);

            if (note == null)
                throw new Exception("Note not found or unauthorized");

            var collaborator = new Collaborator
            {
                Email = dto.Email,
                NoteId = dto.NoteId,
                UserId = user.UserId   // collaborator userId
            };
            var result = _repo.AddCollaborator(collaborator);
            _emailService.SendEmail(
                    dto.Email, "Collaboration Invite",
                    $"You have been added as collaborator on Note ID {dto.NoteId} by user {user.UserId}"
                );
            return result;
        }

        public List<Collaborator> GetCollaborators(int noteId)
        {
            return _repo.GetCollaborators(noteId);
        }

        public bool RemoveCollaborator(int collaboratorId)
        {
            return _repo.RemoveCollaborator(collaboratorId);
        }
    }
}
