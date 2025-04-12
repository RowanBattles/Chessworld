import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { findGame, getMatchStatus } from "../services/api";

const FindGameComponent: React.FC = () => {
  const [statusMessage, setStatusMessage] = useState("");
  const [isError, setIsError] = useState(false);
  const [isDisabled, setIsDisabled] = useState(false);
  const navigate = useNavigate();

  const updateStatus = ({
    msg,
    err = false,
    disabled = true,
  }: {
    msg: string;
    err?: boolean;
    disabled?: boolean;
  }) => {
    setStatusMessage(msg);
    setIsError(err);
    setIsDisabled(disabled);
  };

  const updateStatusError = () => {
    updateStatus({
      msg: "An error occurred while finding a game.",
      err: true,
      disabled: false,
    });
  };

  const pollMatchStatus = async () => {
    const poll = async () => {
      try {
        const response = await getMatchStatus();

        if (response.matchFound) {
          navigate(`/${response.gameId}`);
          return;
        }

        updateStatus({
          msg: response.message,
          err: false,
          disabled: true,
        });
        setTimeout(poll, 500);
      } catch {
        updateStatusError();
      }
    };
    poll();
  };

  const handleFindGame = async () => {
    try {
      updateStatus({ msg: "Searching for a game..." });
      console.log(import.meta.env.VITE_API_URL);
      const response = await findGame();

      if (response.matchFound) {
        navigate(`/${response.gameId}`);
        return;
      }

      updateStatus({ msg: response.message });
      pollMatchStatus();
    } catch {
      updateStatusError();
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
