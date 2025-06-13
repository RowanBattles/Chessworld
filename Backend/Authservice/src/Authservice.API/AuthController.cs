using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Authservice.API
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _repo;
        private readonly EmailService _emailService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _config;

        public AuthController(UserRepository repo, EmailService emailService, ILogger<AuthController> logger, IConfiguration config)
        {
            _repo = repo;
            _emailService = emailService;
            _logger = logger;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var existing = await _repo.GetByUserNameAsync(request.Email);
                if (existing != null)
                {
                    return BadRequest("Email already exists.");
                }

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var user = new UserModel(request.UserName, passwordHash, request.Email);

                _logger.LogInformation("Creating user with email: {Email}", request.Email);
                await _emailService.SendVerificationEmailAsync(user.Email, user.VerificationToken);

                _logger.LogInformation("Sending verification email to: {Email}", request.Email);
                await _repo.CreateUserAsync(user);

                _logger.LogInformation("User registered successfully with email: {Email}", request.Email);
                return Ok("User registered.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration for email: {Email}", request.Email);
                return StatusCode(500, "An error occurred while registering the user.");
            }
        }

        [HttpGet("confirm/{token}")]
        public async Task<IActionResult> VerifyEmail([FromRoute] string token)
        {
            var user = await _repo.GetByVerificationTokenAsync(token);

            if (user == null)
            {
                return BadRequest("Invalid verification token.");
            }

            if (user.EmailVerified)
            {
                return BadRequest("User already verified");
            }

            if (user.VerificationTokenExpiry < DateTime.UtcNow)
            {
                await _repo.DeleteUser(user.Email);
                return BadRequest("Verification token expired. Please register again.");
            }

            user.EmailVerified = true;
            user.VerificationTokenExpiry = null;
            await _repo.UpdateUserAsync(user);

            return Ok("Email verified. You can now log in.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _repo.GetByUserNameAsync(request.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email");
            }

            if (!user.EmailVerified)
            {
                return Unauthorized("Email not verified.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized("Invalid password");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new Exception("No JWT key"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                ]),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            Response.Cookies.Append(
                "jwt",
                jwt,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                }
            );

            return Ok(new { message = "Login successful" });
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                var username = User.FindFirstValue(ClaimTypes.Name);
                return Ok(new { email, username });
            }
            return Unauthorized();
        }
        
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Append("jwt", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(-1)
            });
            return Ok(new { message = "Logged out" });
        }
    }
    public class RegisterRequest
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
    }

    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}