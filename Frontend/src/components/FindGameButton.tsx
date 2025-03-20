import React from "react";
import { useNavigate } from "react-router-dom";
import { findGame, getMatchStatus } from "../services/api";

interface FindGameButtonProps {
  setMessage: (message: string) => void;
  setIsError: (isError: boolean) => void;
}

const FindGameButton: React.FC<FindGameButtonProps> = ({
  setMessage,
  setIsError,
}) => {
  const [loading, setLoading] = React.useState(false);
  const navigate = useNavigate();

  const handleClick = async () => {
    setLoading(true);
    try {
      const response = await findGame();
      setMessage(response.message);
      setIsError(false);

      if (!response.matchFound) {
        pollMatchStatus();
      } else if (response.matchFound && response.gameId) {
        navigate(`/${response.gameId}`);
      }
    } catch (error) {
      setMessage("An error occurred while finding a game.");
      setIsError(true);
    } finally {
      setLoading(false);
    }
  };

  const pollMatchStatus = () => {
    const interval = setInterval(async () => {
      try {
        const response = await getMatchStatus(); // No playerId needed, uses cookie
        if (response.matchFound && response.gameUrl) {
          clearInterval(interval);
          navigate(`/${response.gameUrl}`);
        }
      } catch (error) {
        setMessage("An error occurred while checking match status.");
        clearInterval(interval);
      }
    }, 5000); // Poll every 5 seconds
  };

  return (
    <div className="text-center">
      <button
        onClick={handleClick}
        disabled={loading}
        className="px-4 py-2 bg-blue-500 text-white font-semibold rounded hover:bg-blue-600 disabled:bg-gray-400"
      >
        {loading ? "Loading..." : "Find Game"}
      </button>
    </div>
  );
};

export default FindGameButton;
