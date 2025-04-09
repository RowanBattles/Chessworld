import { useState, useEffect } from "react";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { playerData } from "../types/PlayerType";

let connection: HubConnection | null = null;

const useWebSocket = (gameId: string, playerData?: playerData) => {
  const [error, setError] = useState<string | null>(null);
  const [isReconnecting, setIsReconnecting] = useState(false);

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

        connection.onclose(async () => {
          console.warn(
            "WebSocket connection closed. Attempting to reconnect..."
          );
          setIsReconnecting(true);
          await attemptReconnect();
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
        setError(null); // Clear any previous errors
        setIsReconnecting(false);
      } catch (err) {
        setError("Failed to connect to WebSocket");
        console.error("Error connecting to SignalR:", err);
        setTimeout(() => attemptReconnect(), 5000); // Retry after 5 seconds
      }
    };

    const attemptReconnect = async () => {
      if (!connection) return;
      try {
        await connection.start();
        console.log("Reconnected to WebSocket successfully.");
        setError(null);
        setIsReconnecting(false);
      } catch (err) {
        console.error(
          "Reconnection attempt failed. Retrying in 5 seconds...",
          err
        );
        setTimeout(() => attemptReconnect(), 5000); // Retry after 5 seconds
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

  return { error, connection, isReconnecting };
};

export default useWebSocket;
