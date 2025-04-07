using ChessDotNet;

namespace GameService.API.Business.Services
{
    public class ChessGameHelper
    {
        public Player ConvertStringToPlayerEnum(string? color)
        {
            return color switch
            {
                "white" => Player.White,
                "black" => Player.Black,
                null => Player.None,
                _ => throw new ArgumentException("Invalid color")
            };
        }
    }
}
