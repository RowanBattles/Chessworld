//using Microsoft.AspNetCore.SignalR;

//namespace GameService.API.API.Hubs
//{
//    public class GameHub : Hub
//    {
//        public override async Task onConnectedAsync()
//        {
//            string connectionId = Context.ConnectionId;
//            await base.OnConnectedAsync();
//        }

//        public async Task JoinGame(Guid gameId)
//        {
//            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
//        }
//    }
//}
