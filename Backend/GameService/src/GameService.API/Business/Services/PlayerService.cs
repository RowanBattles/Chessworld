using GameService.API.Data.Repositories;

namespace GameService.API.Business.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerMemoryRepository _playerMemoryRepository;

        public PlayerService(IPlayerMemoryRepository repository)
        {
            _playerMemoryRepository = repository;
        }

        public Guid AddConnectionIdToPlayer(string connectionId)
        {
            Guid playerId = Guid.NewGuid();
            _playerMemoryRepository.AddPlayer(playerId, connectionId);
            return playerId;
        }

        public string? GetConnectionId(Guid playerId)
        {
            return _playerMemoryRepository.GetConnectionId(playerId);
            // TODO: Exception handling
        }

        public Guid? GetPlayerId(string connectionId)
        {
            return _playerMemoryRepository.GetPlayerId(connectionId);
        }
    }
}