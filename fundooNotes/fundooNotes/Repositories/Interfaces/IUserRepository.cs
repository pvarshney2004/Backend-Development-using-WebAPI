using fundooNotes.Models;

namespace fundooNotes.Repositories.Interfaces
{
    // in this interface, we defines what operations are allowed on Users table.
    public interface IUserRepository
    {
        // Insert new user into database
        // Flow: Controller -> Service -> Repository -> Database
        User Register(User user);
        // Check if user already exists
        User GetUserByEmail(string email);
        // Update user information
        void UpdateUser(User user);
        User GetUserByResetToken(string token);
        void CreateSession(UserSession session); // while login
        void Logout(string token);
    }
}
