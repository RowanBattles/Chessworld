import { useState, useEffect } from "react";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { playerData } from "../types/PlayerType";

let connection: HubConnection | null = null;

const useWebSocket = (gameId: string, playerData?: playerData) => {
  if (playerData === undefined) {
    throw new Error("playerData is required");
  }

  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!gameId || !playerData) return;

    const endpoint = playerData.isSpectator ? "/watch" : "/play";

    const url = new URL(`http://localhost:5000${endpoint}`);
    url.searchParams.append("gameId", gameId);
    if (!playerData.isSpectator && playerData.id) {
      url.searchParams.append("token", playerData.id);
    }

    const startConnection = async () => {
      if (!connection) {
        connection = new HubConnectionBuilder().withUrl(url.toString()).build();

        connection.onclose(() => {
          connection = null;
        });
      }

      if (
        connection.state === "Connected" ||
        connection.state === "Connecting"
      ) {
        return;
      }

      try {
        await connection.start();
        console.log(`Connected to SignalR at ${url}`);
      } catch (err) {
        setError("Failed to connect to WebSocket");
        console.error("Error connecting to SignalR:", err);
      }
    };

    startConnection();

    return () => {
      if (connection && connection.state === "Connected") {
        connection
          .stop()
          .then(() => console.log("WebSocket connection stopped"))
          .catch((err) =>
            console.error("Error stopping WebSocket connection:", err)
          );
      }
    };
  }, [gameId, playerData]);

  return { error, connection };
};

export default useWebSocket;
