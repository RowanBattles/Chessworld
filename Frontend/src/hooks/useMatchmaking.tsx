import { useState } from "react";
import { findGame, getMatchStatus } from "../services/api";

export const useMatchmaking = () => {
  const [loading, setLoading] = useState(false);
  const [matchFound, setMatchFound] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [gameUrl, setGameUrl] = useState<string | null>(null);

  const startMatchmaking = async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await findGame();
      if (data.matchFound) {
        setMatchFound(true);
        return data.gameId;
      } else {
        setMatchFound(false);
      }
    } catch (err) {
      setError("Failed to find game");
    } finally {
      setLoading(false);
    }
  };

  const checkMatchStatus = async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await getMatchStatus();
      if (data.matchFound) {
        setGameUrl(data.gameUrl);
      }
    } catch (err) {
      setError("Failed to get match status");
    } finally {
      setLoading(false);
    }
  };

  return {
    loading,
    matchFound,
    error,
    gameUrl,
    startMatchmaking,
    checkMatchStatus,
  };
};
