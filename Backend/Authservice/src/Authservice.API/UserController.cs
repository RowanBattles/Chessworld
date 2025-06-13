using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Authservice.API
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _repo;
        private readonly ILogger<UserController> _logger;

        public UserController(UserRepository repo, ILogger<UserController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("Email not found in token.");
            }

            var user = await _repo.GetByUserNameAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            await _repo.DeleteUser(email);
            _logger.LogInformation("User deleted: {Email}", email);
            return Ok("User deleted.");
        }
    }
}