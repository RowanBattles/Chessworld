import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getGameData } from "../services/api";
import ErrorPage from "./ErrorPage";
import useWebSocket from "../hooks/useWebsocket";
import ChessBoard from "../components/ChessBoard";
import { ErrorType } from "../types/ErrorType";
import { GameType } from "../types/GameType";

const GamePage = () => {
  const { gameId } = useParams<{ gameId: string }>();
  const [gameData, setGameData] = useState<GameType | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<ErrorType | null>(null);

  const { error: websocketError, connection } = useWebSocket(
    gameId || "",
    gameData?.player
  );

  useEffect(() => {
    const fetchGameData = async () => {
      try {
        if (!gameId) {
          throw new Error("Game ID is required");
        }

        const data = await getGameData(gameId);
        setGameData(data);
        console.log("Game data:", data);
        setError(null);
      } catch (err) {
        if (err instanceof Error && "response" in err && err.response) {
          const response = err.response as { status?: number };
          if (response.status === 404) {
            setError({ status: 404, message: "Game not found" });
          } else if (response.status === 500) {
            setError({ status: 500, message: "An unexpected error occurred" });
          } else {
            setError({
              status: response.status || 0,
              message: "An unknown error occurred",
            });
          }
        } else {
          setError({
            status: 0,
            message: "An unknown error occurred",
          });
        }
      } finally {
        setLoading(false);
      }
    };

    fetchGameData();
  }, [gameId]);

  useEffect(() => {
    if (websocketError) {
      console.error("WebSocket error:", websocketError);
    }
  }, [websocketError]);

  if (loading) {
    return <p>Loading...</p>;
  }

  if (error) {
    return <ErrorPage statusCode={error.status} message={error.message} />;
  }

  if (!gameData) {
    return <p>Waiting for game data...</p>;
  }

  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100">
      {websocketError && (
        <p className="text-red-500">
          WebSocket error: {websocketError}. Please refresh the page or try
          again later.
        </p>
      )}
      <div className="w-2/5 bg-white p-4 rounded-lg shadow-md">
        <ChessBoard
          color={gameData.player.color}
          isSpectator={gameData.player.isSpectator}
          fen={gameData.game.fen}
          socket={connection}
        />
      </div>
    </div>
  );
};

export default GamePage;
