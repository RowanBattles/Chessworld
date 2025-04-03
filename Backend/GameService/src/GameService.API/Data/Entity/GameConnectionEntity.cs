using System.ComponentModel.DataAnnotations;

namespace GameService.API.Data.Entity
{
    public class GameConnectionEntity
    {
        [Key]
        public Guid GameId { get; set; }
        public string? ConnectionWhite { get; private set; }
        public string? ConnectionBlack { get; private set; }
        public List<string> ConnectionSpectators { get; private set; }

        public GameConnectionEntity(Guid gameId, string? connectionWhite, string? connectionBlack, List<string> connectionSpectators)
        {
            GameId = gameId;
            ConnectionWhite = connectionWhite;
            ConnectionBlack = connectionBlack;
            ConnectionSpectators = connectionSpectators;
        }
    }
}
