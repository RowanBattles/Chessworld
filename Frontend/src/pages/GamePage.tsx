import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getGameData } from "../services/api"; // Import the API function
import NotFoundComponent from "../components/NotFoundComponent";
import InternalServerErrorComponent from "../components/InternalServerErrorComponent";

type GameData = {
  role: string;
  status: string;
};

const GamePage = () => {
  // Explicitly define the type of gameId as a string
  const { gameId } = useParams<{ gameId: string }>();
  const [gameData, setGameData] = useState<GameData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<{
    status: number;
    message: string;
  } | null>(null);

  useEffect(() => {
    const fetchGameData = async () => {
      try {
        if (!gameId) {
          throw new Error("Game ID is required");
        }

        const data = await getGameData(gameId);
        setGameData(data);
        setError(null);
      } catch (err: any) {
        if (err.response?.status === 404) {
          setError({ status: 404, message: "Game not found" });
        } else if (err.response?.status === 500) {
          setError({ status: 500, message: "An unexpected error occurred" });
        } else {
          setError({
            status: err.response?.status || 0,
            message: "An unknown error occurred",
          });
        }
      } finally {
        setLoading(false);
      }
    };

    fetchGameData();
  }, [gameId]);

  if (loading) {
    return <p>Loading...</p>;
  }

  if (error) {
    if (error.status === 404) {
      return <NotFoundComponent />;
    }
    if (error.status === 500) {
      return <InternalServerErrorComponent />;
    }
    return <p>{error.message}</p>;
  }

  if (!gameData) {
    return <p>Waiting for game data...</p>;
  }

  return (
    <div>
      <p>Your Role: {gameData.role}</p>
      <p>Game Status: {gameData.status}</p>
    </div>
  );
};

export default GamePage;
