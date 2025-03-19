import { useState } from "react";
import { findGame } from "../services/api";

export const useMatchmaking = () => {
  const [loading, setLoading] = useState(false);
  const [matchFound, setMatchFound] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const startMatchmaking = async () => {
    setLoading(true);
    setError(null);

    try {
      const data = await findGame();
      if (data.matchFound) {
        setMatchFound(true);
        // You can also return the gameId if needed
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

  return { loading, matchFound, error, startMatchmaking };
};
