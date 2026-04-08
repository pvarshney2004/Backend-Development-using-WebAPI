using System.ComponentModel.DataAnnotations;

namespace fundooNotes.Models
{
    public class Note
    {
        [Key]
        public int NoteId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Reminder { get; set; }
        public string Colour { get; set; } = "#FFFFFF";
        public string? Image { get; set; }
        public bool IsArchived { get; set; }
        public bool IsPin { get; set; }
        public bool IsTrash { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public ICollection<NoteLabel> NoteLabels { get; set; }
        public ICollection<Collaborator> Collaborators { get; set; }
    }
}
