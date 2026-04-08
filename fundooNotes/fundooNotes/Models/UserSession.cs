using System.ComponentModel.DataAnnotations;

namespace fundooNotes.Models
{
    public class UserSession
    {
        [Key]
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime LogoutTime { get; set; }
        public bool IsActive { get; set; }
    }
}
