using fundooNotes.Models;

namespace fundooNotes.Repositories.Interfaces
{
    public interface ICollaboratorRepository
    {
        Collaborator AddCollaborator(Collaborator collaborator);
        List<Collaborator> GetCollaborators(int noteId);
        bool RemoveCollaborator(int collaboratorId);
    }
}
