namespace Matchmaking.API.Business.Infrastructure
{
    public interface IMatchmakingService
    {
        Guid AddAnonyomousPlayer(string connectionId);
        void UpdatePlayerConnectionId(string playerId, string connectionId);
    }
}
