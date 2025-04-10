import { useState, useEffect } from "react";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";

let connection: HubConnection | null = null;

const useWebSocket = (
  gameId: string,
  playerData?: { id: string; isSpectator: boolean }
) => {
  const [error, setError] = useState<string | null>(null);
  const [isReconnecting, setIsReconnecting] = useState(false);

  useEffect(() => {
    if (!gameId || !playerData) return;
    console.log("here");

    const endpoint = playerData.isSpectator ? "/watch" : "/play";
    const url = new URL(`${(import.meta as any).env.VITE_API_URL}${endpoint}`);
    url.searchParams.append("gameId", gameId);
    if (!playerData.isSpectator && playerData.id) {
      url.searchParams.append("token", playerData.id);
    }

    const startConnection = async () => {
      if (!connection) {
        connection = new HubConnectionBuilder().withUrl(url.toString()).build();

        connection.onclose(async () => {
          setIsReconnecting(true);
          await attemptReconnect();
        });
      }

      if (
        connection.state === "Connected" ||
        connection.state === "Connecting"
      ) {
        console.log("not connected");
        return;
      }

      try {
        await connection.start();
        console.log(`Connected to SignalR at ${url}`);
        setError(null);
        setIsReconnecting(false);
      } catch (err) {
        setError("Failed to connect to WebSocket");
        console.error("Error connecting to SignalR:", err);
        setTimeout(() => attemptReconnect(), 5000);
      }
    };

    const attemptReconnect = async () => {
      if (!connection) return;
      try {
        await connection.start();
        setError(null);
        setIsReconnecting(false);
      } catch (err) {
        console.error(
          "Reconnection attempt failed. Retrying in 5 seconds...",
          err
        );
        setTimeout(() => attemptReconnect(), 5000);
      }
    };

    startConnection();

    return () => {
      console.log("state", connection);
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

  const sendMove = async (move: string) => {
    console.log("send move");
    if (connection?.state === "Connected") {
      try {
        await connection.invoke("MakeMove", move);
        console.log("Move sent:", move);
      } catch (error) {
        console.error("Failed to send move:", error);
      }
    } else {
      console.error("WebSocket connection is not established.");
    }
  };

  const onReceiveMove = (callback: (fen: string) => void) => {
    console.log("receive move");
    if (connection) {
      connection.on("ReceiveMove", callback);
    }
  };

  const offReceiveMove = (callback: (fen: string) => void) => {
    if (connection) {
      connection.off("ReceiveMove", callback);
    }
  };

  return { error, isReconnecting, sendMove, onReceiveMove, offReceiveMove };
};

export default useWebSocket;
