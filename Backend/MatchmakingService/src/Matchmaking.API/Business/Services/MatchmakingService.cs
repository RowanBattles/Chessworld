using Matchmaking.API.Business.Infrastructure;
using Matchmaking.API.Data.Interfaces;

namespace Matchmaking.API.Business.Services
{
    public class MatchmakingService : IMatchmakingService
    {
        private readonly IMatchmakingRepository _matchmakingRepository;

        public MatchmakingService(IMatchmakingRepository matchmakingRepository)
        {
            _matchmakingRepository = matchmakingRepository;
        }

        public Guid AddAnonyomousPlayer(string connectionId)
        {
            Guid playerId = Guid.NewGuid();
            _matchmakingRepository.AddPlayer(playerId, connectionId);
            return playerId;
        }

        public void UpdatePlayerConnectionId(string playerId, string connectionId)
        {
            
        }
    }
}
