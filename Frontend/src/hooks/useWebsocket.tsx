import { useEffect, useMemo, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";

const useWebSocket = (
  gameId: string,
  playerData: { id: string; isSpectator: boolean }
) => {
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const [connectionReady, setConnectionReady] = useState(false);

  const url = useMemo(() => {
    const endpoint = playerData.isSpectator ? "/watch" : "/play";
    const newUrl = new URL(`${import.meta.env.VITE_API_URL}${endpoint}`);
    newUrl.searchParams.append("gameId", gameId);
    if (!playerData.isSpectator) {
      newUrl.searchParams.append("token", playerData.id);
    }
    return newUrl;
  }, [gameId, playerData]);

  useEffect(() => {
    let isMounted = true;

    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(url.toString())
      .withAutomaticReconnect()
      .build();

    const connect = async () => {
      const existingConn = connectionRef.current;

      if (
        existingConn &&
        existingConn.state !== signalR.HubConnectionState.Disconnected
      ) {
        try {
          await existingConn.stop();
        } catch (stopErr) {
          console.error("Failed to stop previous connection:", stopErr);
        }
      }

      try {
        await newConnection.start();
        if (isMounted) {
          connectionRef.current = newConnection;
          setConnectionReady(true);
        }
      } catch (startErr) {
        console.error("WebSocket connection failed:", startErr);
        if (isMounted) setConnectionReady(false);
      }
    };

    connect();

    return () => {
      isMounted = false;
      newConnection
        .stop()
        .catch((err) => console.error("Error disconnecting WebSocket:", err));

      setConnectionReady(false);
    };
  }, [url]);

  const sendMove = async (move: string) => {
    const conn = connectionRef.current;
    if (conn && conn.state === signalR.HubConnectionState.Connected) {
      try {
        await conn.invoke("MakeMove", move);
      } catch (err) {
        console.error("Failed to send move:", err);
      }
    } else {
      console.error("WebSocket is not connected.");
    }
  };

  const receiveMove = (callback: (fen: string) => void) => {
    const conn = connectionRef.current;
    if (conn) {
      conn.on("ReceiveMove", callback);
    } else {
      console.error("WebSocket is not connected.");
    }
  };

  return { sendMove, receiveMove, connectionReady };
};

export default useWebSocket;
