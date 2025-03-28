import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { findGame, getMatchStatus } from "../services/api";

const FindGameComponent: React.FC = () => {
  const [statusMessage, setStatusMessage] = useState<string>("");
  const [isError, setIsError] = useState<boolean>(false);
  const [isDisabled, setIsDisabled] = useState(false);
  const navigate = useNavigate();

  const pollMatchStatus = async () => {
    const poll = async () => {
      try {
        const response = await getMatchStatus();
        console.log("polling :", response);
        setStatusMessage(response.message);

        if (response.matchFound) {
          setStatusMessage("Match found!");
          navigate(`/${response.gameId}`);
          return;
        }

        setTimeout(poll, 500);
      } catch (error) {
        console.error("Error while polling match status:", error);
        setStatusMessage("An error occurred while checking match status.");
        setIsError(true);
        setIsDisabled(false);
      }
    };

    poll();
  };

  const handleFindGame = async () => {
    try {
      setIsDisabled(true);
      setIsError(false);
      setStatusMessage("Searching for a game...");
      const response = await findGame();
      console.log("first response: ", response);
      setStatusMessage(response.message);

      if (!response.matchFound) {
        pollMatchStatus();
      } else {
        setStatusMessage("Match found!");
        navigate(`/${response.gameId}`);
      }
    } catch (error) {
      console.error("Error while finding a game:", error);
      setStatusMessage("An error occurred while finding a game.");
      setIsError(true);
      setIsDisabled(false);
    }
  };

  return (
    <div className="flex flex-col items-center justify-center pb-[10%] min-h-screen bg-gray-100">
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
      <p
        className={`mt-4 min-h-[1.5rem] ${
          isError ? "text-red-500" : "text-gray-700"
        }`}
      >
        {statusMessage}
      </p>
    </div>
  );
};

export default FindGameComponent;
