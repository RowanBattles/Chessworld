using GameService.API.Data.Repositories;

namespace GameService.API.Business.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerMemoryRepository;

        public PlayerService(IPlayerRepository repository)
        {
            _playerMemoryRepository = repository;
        }

        public Guid AddConnectionId(string connectionId)
        {
            Guid playerId = Guid.NewGuid();
            _playerMemoryRepository.AddPlayer(playerId, connectionId);
            return playerId;
        }

        public Guid? RemoveConnectionId(string connectionId)
        {
            var playerId = GetPlayerId(connectionId);
            _playerMemoryRepository.RemovePlayer(connectionId);
            return playerId;
        }

        public string? GetConnectionId(Guid playerId)
        {
            return _playerMemoryRepository.GetConnectionId(playerId);
        }

        private Guid? GetPlayerId(string connectionId)
        {
            return _playerMemoryRepository.GetPlayerId(connectionId);
        }
    }
}