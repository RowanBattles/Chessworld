import React, { useState } from "react";
import { findGame, getMatchStatus } from "../services/api";

const FindGameComponent: React.FC = () => {
  const [statusMessage, setStatusMessage] = useState<string>("");
  const [isDisabled, setIsDisabled] = useState(false);

  const pollMatchStatus = async () => {
    const poll = async () => {
      try {
        const response = await getMatchStatus();
        console.log(response);

        if (response.matchFound) {
          setStatusMessage("Match found!");
          setIsDisabled(false);
          return;
        }

        setTimeout(poll, 500);
      } catch (error) {
        console.error("Error while polling match status:", error);
        setStatusMessage("An error occurred while checking match status.");
        setIsDisabled(false);
      }
    };

    poll();
  };

  const handleFindGame = async () => {
    try {
      setIsDisabled(true);
      setStatusMessage("Searching for a game...");
      const response = await findGame();
      console.log(response);
      setStatusMessage(response.message);

      if (!response.matchFound) {
        pollMatchStatus();
        const initialStatus = await getMatchStatus();
        console.log(initialStatus);
      }
    } catch (error) {
      setStatusMessage("An error occurred while finding a game.");
      setIsDisabled(false);
    }
  };

  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100">
      <h1 className="text-2xl font-bold mb-4">Find Game</h1>
      <button
        onClick={handleFindGame}
        disabled={isDisabled}
        className={`px-4 py-2 rounded transition text-white ${
          isDisabled
            ? "bg-gray-400 cursor-not-allowed"
            : "bg-blue-500 hover:bg-blue-600"
        }`}
      >
        Find Game
      </button>
      <p className="text-gray-700 mt-4 min-h-[1.5rem]">{statusMessage}</p>
    </div>
  );
};

export default FindGameComponent;
