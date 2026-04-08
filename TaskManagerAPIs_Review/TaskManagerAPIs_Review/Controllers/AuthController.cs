using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPIs_Review.DTOs.Auth;
using TaskManagerAPIs_Review.Services.Interface;

namespace TaskManagerAPIs_Review.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        [HttpPost("register")]
        public IActionResult Register(RegisterDTO registerDTO)
        {
            try
            {
                _logger.LogInformation("Register API called for Username: {Username}", registerDTO.Username);
                var result = _authService.Register(registerDTO);
                if (result != null)
                {
                    _logger.LogInformation("User registered successfully: {Username}", registerDTO.Username);
                    return Ok(result);
                }
                _logger.LogWarning("Registration failed for Username: {Username}", registerDTO.Username);
                return BadRequest("Registration failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for Username: {Username}", registerDTO.Username);
                return StatusCode(500, "Internal server error");

            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDTO loginDTO)
        {
            var token = _authService.Login(loginDTO);
            if (token != null)
            {
                return Ok(new { Token = token });
            }
            return BadRequest("Invalid username or password");
        }
    }
}
