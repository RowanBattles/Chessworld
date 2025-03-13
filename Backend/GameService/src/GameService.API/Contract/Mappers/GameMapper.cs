using GameService.API.API.Responses;
using GameService.API.Business.Models;
using GameService.API.Contract.Enums;
using GameService.API.Data.Entity;

namespace GameService.API.Contract.Mappers
{
    public static class GameMapper
    {
        public static GameModel ToGameModel(GameRequestModel requestModel)
        {
            var white = requestModel.Player1Color.ToLower() == "white" ? requestModel.Player1Id : requestModel.Player2Id;
            var black = requestModel.Player1Color.ToLower() == "black" ? requestModel.Player1Id : requestModel.Player2Id;

            return new GameModel(
                requestModel.GameId,
                white,
                black,
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