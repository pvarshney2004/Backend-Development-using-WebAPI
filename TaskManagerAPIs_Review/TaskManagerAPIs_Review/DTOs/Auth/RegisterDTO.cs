namespace TaskManagerAPIs_Review.DTOs.Auth
{
    public class RegisterDTO
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
    }
}
