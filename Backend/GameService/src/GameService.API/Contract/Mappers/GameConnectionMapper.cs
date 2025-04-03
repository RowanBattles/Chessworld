using GameService.API.Business.Models;
using GameService.API.Data.Entity;

namespace GameService.API.Contract.Mappers
{
    public static class GameConnectionMapper
    {
        public static GameConnectionEntity ToEntity(GameConnectionModel model)
        {
            return new GameConnectionEntity(
                model.GameId,
                model.ConnectionWhite,
                model.ConnectionBlack,
                model.ConnectionSpectators
            );
        }

        public static GameConnectionModel ToModel(GameConnectionEntity entity)
        {
            return new GameConnectionModel(
                entity.GameId,
                entity.ConnectionWhite,
                entity.ConnectionBlack,
                entity.ConnectionSpectators
            );
        }
    }
}
