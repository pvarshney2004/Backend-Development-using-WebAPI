using fundooNotes.Models;
using fundooNotes.DTOs.Auth;

namespace fundooNotes.Services.Interfaces
{
    public interface IUserService
    {
        User Register(RegisterDTO registerDTO);
        string Login(LoginDTO loginDTO);
        bool ForgotPassword(string email);
        bool ResetPassword(string token, string newPassword);
        void Logout(string token);
    }
}
