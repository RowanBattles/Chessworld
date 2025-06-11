using Microsoft.AspNetCore.Mvc;

namespace Authservice.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _repo;
        private readonly EmailService _emailService;

        public UserController(UserRepository repo, EmailService emailService)
        {
            _repo = repo;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var existing = await _repo.GetByUserNameAsync(request.Email);
            if (existing != null)
                return BadRequest("Email already exists.");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new UserModel(request.UserName, passwordHash, request.Email);
            await _repo.CreateUserAsync(user);

            await _emailService.SendVerificationEmailAsync(user.Email, user.VerificationToken);

            return Ok("User registered.");
        }

        [HttpGet("confirm/{token}")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var user = await _repo.GetByVerificationTokenAsync(token);
            if (user == null || user.VerificationTokenExpiry < DateTime.UtcNow)
                return BadRequest("Invalid or expired verification token.");

            user.EmailVerified = true;
            user.VerificationToken = null;
            user.VerificationTokenExpiry = null;
            await _repo.UpdateUserAsync(user);

            return Ok("Email verified. You can now log in.");
        }
    }
    public class RegisterRequest
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
    }
}