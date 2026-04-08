using TaskManagerAPIs_Review.DTOs.Auth;
using TaskManagerAPIs_Review.Models;

namespace TaskManagerAPIs_Review.Services.Interface
{
    public interface IAuthService
    {
        User Register(RegisterDTO registerDTO);
        string Login(LoginDTO loginDTO);
    }
}
