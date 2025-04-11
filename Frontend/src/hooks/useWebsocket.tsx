import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";

const useWebSocket = (
  gameId: string,
  playerData: { id: string; isSpectator: boolean }
) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null
  );

  useEffect(() => {
    const connect = async () => {
      const endpoint = playerData.isSpectator ? "/watch" : "/play";
      const url = new URL(
        `${(import.meta as any).env.VITE_API_URL}${endpoint}`
      );
      url.searchParams.append("gameId", gameId);
      if (!playerData.isSpectator) {
        url.searchParams.append("token", playerData.id);
      }

      const newConnection = new signalR.HubConnectionBuilder()
        .withUrl(url.toString())
        .withAutomaticReconnect()
        .build();

      try {
        await newConnection.start();
        console.log("WebSocket connected.");
        setConnection(newConnection);
      } catch (err) {
        console.error("WebSocket connection failed:", err);
      }
    };

    connect();

    return () => {
      if (connection) {
        connection
          .stop()
          .then(() => console.log("WebSocket disconnected."))
          .catch((err) => console.error("Error disconnecting WebSocket:", err));
      }
    };
  }, [gameId, playerData]);

  const sendMove = async (move: string) => {
    if (
      connection &&
      connection.state === signalR.HubConnectionState.Connected
    ) {
      try {
        await connection.invoke("MakeMove", move);
        console.log("Move sent:", move);
      } catch (err) {
        console.error("Failed to send move:", err);
      }
    } else {
      console.error("WebSocket is not connected.");
    }
  };

  const receiveMove = (callback: (fen: string) => void) => {
    if (connection) {
      connection.on("ReceiveMove", callback);
    } else {
      console.error("WebSocket is not connected.");
    }
  };

  return { sendMove, receiveMove };
};

export default useWebSocket;
