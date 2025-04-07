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
                return await _gameRepository.GetGameByPlayerId(playerToken);
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

        public async Task MakeMove(Guid gameId, string? color, string uci)
        {
            try
            {
                GameModel gameModel = await _gameRepository.GetGameByGameId(gameId) ?? throw new KeyNotFoundException("Game not found");

                ChessGame board = new(gameModel.Fen);

                Player player = new ChessGameHelper().ConvertStringToPlayerEnum(color);

                Move move = new(uci[..2], uci.Substring(2, 2), player);

                MoveType moveType = board.MakeMove(move, false);

                if (moveType == MoveType.Invalid)
                {
                    throw new InvalidOperationException("Invalid move");
                }

                gameModel.UpdateBoard(board.GetFen());

                await _gameRepository.UpdateGame(gameModel);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Game not found for gameId");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid move");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making move");
                throw new InvalidOperationException("Move could not be made", ex);
            }
        }
    }
}