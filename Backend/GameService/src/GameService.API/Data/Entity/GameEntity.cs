using System.ComponentModel.DataAnnotations;

namespace GameService.API.Data.Entity
{
    public class GameEntity
    {
        [Key]
        public Guid Id { get; private set; }

        [Required]
        public Guid White { get; private set; }

        [Required]
        public Guid Black { get; private set; }

        [Required]
        public int Status { get; private set; }

        public GameEntity(Guid id, Guid white, Guid black, int status)
        {
            Id = id;
            White = white;
            Black = black;
            Status = status;
        }
    }
}
