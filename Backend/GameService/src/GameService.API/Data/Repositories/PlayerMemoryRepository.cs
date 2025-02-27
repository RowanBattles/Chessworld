using System.Collections.Concurrent;

namespace GameService.API.Data.Repositories
{
    public class PlayerMemoryRepository : IPlayerMemoryRepository
    {
        private readonly ConcurrentDictionary<Guid, string> _playerConnections = new();

        public void AddPlayer(Guid playerId, string connectionId)
        {
            _playerConnections[playerId] = connectionId;
        }

        public string? GetConnectionId(Guid playerId)
        {
            _playerConnections.TryGetValue(playerId, out var connectionId);
            return connectionId;
        }

        public Guid? GetPlayerId(string connectionId)
        {
            return _playerConnections.FirstOrDefault(kvp => kvp.Value == connectionId).Key;
        }
    }
}