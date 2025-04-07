namespace GameService.API.API.Responses
{
    public class ConnectionStatusResponse
    {
        public bool White {  get; private set; }
        public bool Black { get; private set; }
        public int Watchers { get; private set; }

        public ConnectionStatusResponse(bool white, bool black, int watchers)
        {
            White = white;
            Black = black;
            Watchers = watchers;
        }
    }
}
