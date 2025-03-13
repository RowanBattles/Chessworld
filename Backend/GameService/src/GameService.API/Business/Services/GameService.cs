using GameService.API.API.Responses;
using GameService.API.Business.Interfaces;
using GameService.API.Business.Models;
using GameService.API.Contract.Mappers;

namespace GameService.API.Business.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public Task<Guid> CreateGame(GameModel gameModel)
        {
            bool gameCreatedSuccesfully = _gameRepository.AddGame(gameModel);
            
            if (gameCreatedSuccesfully)
            {
                // Create websocket connection and cookie for black and white
            }
            else
            {
                // Handle exception
            }

            return null;
        }
    }
}
