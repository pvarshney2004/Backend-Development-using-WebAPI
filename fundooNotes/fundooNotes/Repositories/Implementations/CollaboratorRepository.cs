using fundooNotes.Context;
using fundooNotes.Models;
using fundooNotes.Repositories.Interfaces;

namespace fundooNotes.Repositories.Implementations
{
    public class CollaboratorRepository : ICollaboratorRepository
    {
        private readonly FundooContext _context;

        public CollaboratorRepository(FundooContext context)
        {
            _context = context;
        }
        public Collaborator AddCollaborator(Collaborator collaborator)
        {
            _context.Collaborators.Add(collaborator);
            _context.SaveChanges();
            return collaborator;
        }

        public List<Collaborator> GetCollaborators(int noteId)
        {
            return _context.Collaborators
                   .Where(c => c.NoteId == noteId)
                   .ToList();
        }

        public bool RemoveCollaborator(int collaboratorId)
        {
            var collaborator = _context.Collaborators.Find(collaboratorId);

            if (collaborator == null)
                return false;

            _context.Collaborators.Remove(collaborator);
            _context.SaveChanges();
            return true;
        }
    }
}
