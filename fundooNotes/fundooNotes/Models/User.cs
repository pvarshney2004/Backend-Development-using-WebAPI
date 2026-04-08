using System.ComponentModel.DataAnnotations;

namespace fundooNotes.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string? ResetToken { get; set; }

        // navigation properties
        public ICollection<Note> Notes { get; set; }
        public ICollection<Label> Labels { get; set; }
        public ICollection<Collaborator> Collaborators { get; set; }
    }
}
