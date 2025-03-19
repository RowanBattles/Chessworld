import React from "react";
import { useNavigate } from "react-router-dom";
import { findGame, getMatchStatus } from "../services/api";

interface FindGameButtonProps {
  setMessage: (message: string) => void;
}

const FindGameButton: React.FC<FindGameButtonProps> = ({ setMessage }) => {
  const [loading, setLoading] = React.useState(false);
  const navigate = useNavigate();

  const handleClick = async () => {
    setLoading(true);
    try {
      const response = await findGame();
      setMessage(response.message); // Pass the message to Homepage

      if (!response.matchFound && response.playerId) {
        // Start polling the matchstatus endpoint
        pollMatchStatus(response.playerId);
      } else if (response.matchFound && response.gameId) {
        // Redirect to the game page if match is already found
        navigate(`/game/${response.gameId}`);
      }
    } catch (error) {
      setMessage("An error occurred while finding a game.");
    } finally {
      setLoading(false);
    }
  };

  const pollMatchStatus = (playerId: string) => {
    const interval = setInterval(async () => {
      try {
        const response = await getMatchStatus(playerId);
        if (response.matchFound && response.gameUrl) {
          clearInterval(interval); // Stop polling
          navigate(`/game/${response.gameUrl}`); // Redirect to the game page
        }
      } catch (error) {
        setMessage("An error occurred while checking match status.");
        clearInterval(interval); // Stop polling on error
      }
    }, 500); // Poll every 500ms
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
