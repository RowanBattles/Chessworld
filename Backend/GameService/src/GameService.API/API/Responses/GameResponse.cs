using Microsoft.AspNetCore.Identity;

namespace GameService.API.API.Responses
{
    public class GameResponse
    {
        public string Role { get; set; }
        public string Status { get; set; }

        public GameResponse(string role, string status)
        {
            Role = role;
            Status = status;
        }
    }
}
