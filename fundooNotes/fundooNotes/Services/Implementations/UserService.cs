using fundooNotes.DTOs.Auth;
using fundooNotes.Helpers;
using fundooNotes.Models;
using fundooNotes.Repositories.Interfaces;
using fundooNotes.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
namespace fundooNotes.Services.Implementations
{
    public class UserService : IUserService
    {
        // this class will handle all the business logic related to user operations
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenGenerator _jwtToken;
        public UserService(IUserRepository userRepository, JwtTokenGenerator jwtToken)
        {
            _userRepository = userRepository;
            _jwtToken = jwtToken;
        }
        // this method will handle user registration logic
        public User Register(RegisterDTO registerDTO)
        {
            var existingUser = _userRepository.GetUserByEmail(registerDTO.Email);
            if (existingUser != null)
            {
                throw new Exception("User already exists");
            }
            string hashedPassword = HashPassword(registerDTO.Password);
            User user = new User
            {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Email = registerDTO.Email,
                Password = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };
            return _userRepository.Register(user);
        }

        // this method will handle user login logic
        public string Login(LoginDTO loginDTO)
        {
            var user = _userRepository.GetUserByEmail(loginDTO.Email);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            string hashedPassword = HashPassword(loginDTO.Password);
            if (user.Password != hashedPassword)
            {
                throw new Exception("Invalid email or password");
            }

            var token = _jwtToken.GenerateToken(user.UserId, user.Email, user.FirstName);
            var session = new UserSession
            {
                UserId = user.UserId,
                Token = token,
                LoginTime = DateTime.Now,  
                IsActive = true
            };

            _userRepository.CreateSession(session);
            return token;
            //return "Login succesful";
        }

        public bool ForgotPassword(string email)
        {
            var user = _userRepository.GetUserByEmail(email);

            if (user == null)
                return false;

            // generate reset token
            var token = Guid.NewGuid().ToString();
            user.ResetToken = token;

            _userRepository.UpdateUser(user);

            Console.WriteLine($"Reset Token: {token}");
            return true;
        }

        public bool ResetPassword(string token, string newPassword)
        {
            var user = _userRepository.GetUserByResetToken(token);

            if (user == null)
                return false;

            user.Password = HashPassword(newPassword);
            user.ResetToken = null;
            _userRepository.UpdateUser(user);

            return true;
        }

        private string HashPassword(string password)
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

        public void Logout(string token)
        {
            _userRepository.Logout(token);
        }
    }
}
