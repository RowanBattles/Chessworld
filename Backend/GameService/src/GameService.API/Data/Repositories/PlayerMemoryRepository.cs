using System.Collections.Concurrent;

namespace GameService.API.Data.Repositories
{
    public class PlayerMemoryRepository : IPlayerRepository
    {
        private readonly ConcurrentDictionary<Guid, string> _playerConnections = new();

        public void AddPlayer(Guid playerId, string connectionId)
        {
            _playerConnections[playerId] = connectionId;
        }

        public void RemovePlayer(string connectionId)
        {
            var playerId = _playerConnections.FirstOrDefault(kvp => kvp.Value == connectionId).Key;
            if (playerId != Guid.Empty)
            {
                _playerConnections.TryRemove(playerId, out _);
            }
        }

        public string? GetConnectionId(Guid playerId)
        {
            _playerConnections.TryGetValue(playerId, out var connectionId);
            return connectionId;
        }

        public Guid? GetPlayerId(string connectionId)
        {
            var playerId = _playerConnections.FirstOrDefault(kvp => kvp.Value == connectionId).Key;
            return playerId == Guid.Empty ? (Guid?)null : playerId;
        }
    }
}