using ChessDotNet;
using GameService.API.Business.Interfaces;
using GameService.API.Business.Models;

namespace GameService.API.Business.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<GameService> _logger;

        public GameService(IGameRepository gameRepository, ILogger<GameService> logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public async Task CreateGame(GameModel gameModel)
        {
            try
            {
                await _gameRepository.AddGame(gameModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding game to repository");
                throw new Exception("Game could not be created", ex);
            }
        }

        public async Task<Guid> GetStatusByPlayerId(string playerToken)
        {
            try
            {
                Guid gameId = await _gameRepository.GetGameByPlayerId(playerToken);
                return gameId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving game for playerToken");
                throw new Exception("Game could not be retrieved", ex);
            }
        }

        public async Task<(string, string, string?, string)> GetGameByGameId(string? playerToken, Guid gameId)
        {
            try
            {
                var gameModel = await _gameRepository.GetGameByGameId(gameId) ?? throw new KeyNotFoundException("Game not found");

                string color = playerToken == gameModel.WhiteToken ? "white" :
                                playerToken == gameModel.BlackToken ? "black" :
                                "white";

                string? validToken = playerToken == gameModel.WhiteToken ? gameModel.WhiteToken :
                                    playerToken == gameModel.BlackToken ? gameModel.BlackToken :
                                    null;

                return (gameModel.Status.ToString(), gameModel.Fen, validToken, color);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Game not found for gameId");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving game for gameId");
                throw new InvalidOperationException("Game could not be retrieved", ex);
            }
        }

        public async Task<string> MakeMove(Guid gameId, string? color, string uci)
        {
            if (gameId == Guid.Empty)
            {
                _logger.LogWarning("Invalid gameId provided");
                throw new ArgumentException("GameId cannot be empty", nameof(gameId));
            }

            if (string.IsNullOrWhiteSpace(color))
            {
                _logger.LogWarning("Invalid color provided");
                throw new ArgumentException("Color cannot be null or empty", nameof(color));
            }

            if (string.IsNullOrWhiteSpace(uci) || uci.Length < 4 || uci.Length > 5)
            {
                _logger.LogWarning("Invalid UCI move provided");
                throw new ArgumentException("UCI move must be 4 or 5 characters", nameof(uci));
            }

            try
            {
                GameModel gameModel = await _gameRepository.GetGameByGameId(gameId).ConfigureAwait(false)
                    ?? throw new KeyNotFoundException($"Game with ID {gameId} not found");

                ChessGame board = new(gameModel.Fen);

                Player player = new ChessGameHelper().ConvertStringToPlayerEnum(color);

                string from = uci[..2];
                string to = uci.Substring(2, 2);
                char? promotion = uci.Length == 5 ? uci[4] : null;

                Move move = promotion.HasValue
                    ? new Move(from, to, player, promotion.Value)
                    : new Move(from, to, player);

                MoveType moveType = board.MakeMove(move, false);
                if (moveType == MoveType.Invalid)
                {
                    _logger.LogWarning("Invalid move attempted: {Uci} by {Color} in game {GameId}", uci, color, gameId);
                    throw new InvalidOperationException("The move is invalid");
                }

                string updatedFen = board.GetFen();
                gameModel.UpdateBoard(updatedFen);

                _logger.LogInformation("Persisting updated FEN: {Fen} for game {GameId}", updatedFen, gameId);
                await _gameRepository.UpdateGame(gameModel);
                return updatedFen;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Game not found for gameId: {GameId}", gameId);
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid move attempted in game {GameId}", gameId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while making a move in game {GameId}", gameId);
                throw new InvalidOperationException("An error occurred while making the move", ex);
            }
        }

        public async Task<List<GameModel>> GetAllGames()
        {
            try
            {
                return await _gameRepository.GetAllGames();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving all games");
                throw new InvalidOperationException("An error occurred while making the move", ex);
            }
        }
    }
}