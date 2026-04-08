using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace fundooNotes.Models
{
    public class Collaborator
    {
        [Key]
        public int CollaboratorId { get; set; }
        public string Email { get; set; }

        public int NoteId { get; set; }
        public int UserId { get; set; }

        // Navigation properties
        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public Note Note { get; set; }
    }
}
