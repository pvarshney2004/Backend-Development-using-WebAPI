using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Cryptography;
using System.Text;
using TaskManagerAPIs_Review.DTOs.Auth;
using TaskManagerAPIs_Review.Helpers;
using TaskManagerAPIs_Review.Models;
using TaskManagerAPIs_Review.Repositories.Interface;
using TaskManagerAPIs_Review.Services.Interface;

namespace TaskManagerAPIs_Review.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        public AuthService(IAuthRepository authRepository, JwtTokenGenerator jwtTokenGenerator)
        {
            _authRepository = authRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public User Register(RegisterDTO registerDTO)
        {
            var userExists = _authRepository.GetUserByUsername(registerDTO.Username);
            if(userExists != null)
            {
                throw new Exception("User not exists");
            }
            string hashedPassword = HashPassword(registerDTO.PasswordHash);
            User user = new User
            {
                Username = registerDTO.Username,
                PasswordHash = hashedPassword,
                Role = registerDTO.Role
            };
            return _authRepository.Register(user);
        }

        public string Login(LoginDTO loginDTO)
        {
            var userExist = _authRepository.GetUserByUsername(loginDTO.Username);
            if(userExist == null)
            {
                throw new Exception("User not found");
            }
            string hashedPass = HashPassword(loginDTO.Password);
            if(userExist.PasswordHash != hashedPass)
            {
                throw new Exception("Invalid password");
            }
            var token = _jwtTokenGenerator.GenerateToken(userExist.Id, userExist.Username, userExist.Role);
            return token;
        }
        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
