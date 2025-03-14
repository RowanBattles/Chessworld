using GameService.API.Business.Models;
using GameService.API.Contract.Enums;
using GameService.API.Data.Entity;

namespace GameService.API.Contract.Mappers
{
    public static class GameMapper
    {
        public static GameModel ToGameModel(Guid whiteId, Guid blackId)
        {
            return new GameModel(
                Guid.NewGuid(),
                whiteId,
                blackId,
                GameStatus.InProgress
            );
        }

        public static GameEntity ToGameEntity(GameModel model)
        {
            return new GameEntity(
                model.Id,
                model.White,
                model.Black,
                (int)model.Status
            );
        }

        public static GameModel ToGameModel(GameEntity entity)
        {
            return new GameModel(
                entity.Id,
                entity.White,
                entity.Black,
                (GameStatus)entity.Status
            );
        }
    }
}