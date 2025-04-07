import { useState, useEffect } from "react";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";

let connection: HubConnection | null = null;

const useWebSocket = (gameId: string, playerData: any) => {
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!gameId || !playerData) return;

    const endpoint = playerData.isSpectator ? "/watch" : "/play";

    const url = new URL(`http://localhost:8080${endpoint}`);
    url.searchParams.append("gameId", gameId);
    if (!playerData.isSpectator && playerData.id) {
      url.searchParams.append("token", playerData.id);
    }

    const startConnection = async () => {
      if (!connection) {
        console.log("Connecting to:", url.toString());
        connection = new HubConnectionBuilder().withUrl(url.toString()).build();

        connection.onclose(() => {
          console.log("WebSocket connection closed.");
          connection = null;
        });
      }

      if (
        connection.state === "Connected" ||
        connection.state === "Connecting"
      ) {
        console.log("Connection is already in progress or established.");
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
