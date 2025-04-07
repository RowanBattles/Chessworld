using GameService.API.Business.Models;
using GameService.API.Contract.Enums;
using GameService.API.Data.Entity;

namespace GameService.API.Contract.Mappers
{
    public static class GameMapper
    {
        public static GameModel ToGameModel(string whiteToken, string blackToken)
        {
            return new GameModel(
                Guid.NewGuid(),
                whiteToken,
                blackToken
            );
        }

        public static GameEntity ToGameEntity(GameModel model)
        {
            return new GameEntity(
                model.Id,
                model.WhiteToken,
                model.BlackToken,
                (int)model.Status,
                model.Fen
            );
        }

        public static GameModel ToGameModel(GameEntity entity)
        {
            return new GameModel(
                entity.Id,
                entity.WhiteToken,
                entity.BlackToken,
                (GameStatus)entity.Status,
                entity.Fen
            );
        }
    }
}

