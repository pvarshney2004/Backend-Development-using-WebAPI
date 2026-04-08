using System.ComponentModel.DataAnnotations;

namespace fundooNotes.Models
{
    public class Label
    {
        [Key]
        public int LabelId { get; set; }
        public string LabelName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UserId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public List<NoteLabel> NoteLabels { get; set; }
    }
}
