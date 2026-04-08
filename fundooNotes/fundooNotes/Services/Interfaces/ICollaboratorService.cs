using fundooNotes.DTOs.Collaborators;
using fundooNotes.Models;

namespace fundooNotes.Services.Interfaces
{
    public interface ICollaboratorService
    {
        Collaborator AddCollaborator(AddCollaboratorDTO dto, int userId);
        List<Collaborator> GetCollaborators(int noteId);
        bool RemoveCollaborator(int collaboratorId);
    }
}
