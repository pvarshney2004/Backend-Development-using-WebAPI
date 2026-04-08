namespace fundooNotes.DTOs.Notes
{
    public class CreateNoteDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? Reminder { get; set; }
    }
}
