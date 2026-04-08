namespace fundooNotes.DTOs.Notes
{
    public class NoteResponseDTO
    {
        public int NoteId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPin { get; set; }
        public bool IsArchived { get; set; }
        public bool IsTrash { get; set; }
        public string Colour { get; set; }
        public DateTime? Reminder { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
