using fundooNotes.DTOs.Auth;
using fundooNotes.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fundooNotes.Controllers
{
    [Route("api/[controller]")]   // /api/User/register
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger; // Logger for tracking API calls and errors
        // ILogger → logging interface
        //<UserController> → the category name for the logs
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDTO registerDTO)
        {
            try
            {
                _logger.LogInformation("Register API called for Email: {Email}", registerDTO.Email);
                var result = _userService.Register(registerDTO);//
                if (result != null)
                {
                    _logger.LogInformation("User registered successfully: {Email}", registerDTO.Email);
                    return Ok(result);
                }
                _logger.LogWarning("Registration failed for Email: {Email}", registerDTO.Email);
                return BadRequest("Registration failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for Email: {Email}", registerDTO.Email);
                return StatusCode(500, "Internal server error");

            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDTO loginDTO)
        {
            try
            {
                _logger.LogInformation("Login API called for Email: {Email}", loginDTO.Email);
                var token = _userService.Login(loginDTO);
                if (token != null)
                {
                    _logger.LogInformation("User logged in successfully: {Email}", loginDTO.Email);
                    return Ok(new { Token = token });
                }
                _logger.LogWarning("Login failed for Email: {Email}", loginDTO.Email);
                return BadRequest("Invalid email or password");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for Email: {Email}", loginDTO.Email);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(ForgotPasswordDTO dto)
        {
            try
            {
                _logger.LogInformation("Forgot Password API called for Email: {Email}", dto.Email);
                var result = _userService.ForgotPassword(dto.Email);
                if (result)
                {
                    _logger.LogInformation("Password reset link sent successfully for Email: {Email}", dto.Email);
                    return Ok("Password reset link sent");
                }
                _logger.LogWarning("Email not found for Forgot Password: {Email}", dto.Email);
                return NotFound("Email not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during Forgot Password for Email: {Email}", dto.Email);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(ResetPasswordDTO dto)
        {
            try
            {
                _logger.LogInformation("Reset Password API called with Token: {Token}", dto.Token);
                var result = _userService.ResetPassword(dto.Token, dto.NewPassword);
                if (result)
                {
                    _logger.LogInformation("Password reset successful for Token: {Token}", dto.Token);
                    return Ok("Password reset successful");
                }
                _logger.LogWarning("Invalid token for Reset Password: {Token}", dto.Token);
                return BadRequest("Invalid token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during Reset Password for Token: {Token}", dto.Token);
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"]
                        .ToString()
                        .Replace("Bearer ", "");

            _userService.Logout(token);
            return Ok(new
            {
                success = true,
                message = "User logged out successfully"
            });
        }


    }
}
