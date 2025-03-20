using System.ComponentModel.DataAnnotations;

namespace GameService.API.Data.Entity
{
    public class GameEntity
    {
        [Key]
        public Guid Id { get; private set; }

        [Required]
        public string WhiteToken { get; private set; }

        [Required]
        public string BlackToken { get; private set; }

        [Required]
        public int Status { get; private set; }

        public GameEntity(Guid id, string whiteToken, string blackToken, int status)
        {
            Id = id;
            WhiteToken = whiteToken;
            BlackToken = blackToken;
            Status = status;
        }
    }
}
