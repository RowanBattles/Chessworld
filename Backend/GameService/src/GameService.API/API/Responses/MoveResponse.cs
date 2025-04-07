namespace GameService.API.API.Responses
{
    public class MoveResponse
    {
        public string Uci { get; private set; }
        public string Fen { get; private set; }
        public int MoveNb { get; private set; }

        public MoveResponse(string uci, string fen, int moveNb)
        {
            Uci = uci;
            Fen = fen;
            MoveNb = moveNb;
        }
    }
}
