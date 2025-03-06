import React, { createContext, useContext, useState, useEffect } from "react";
import * as signalR from "@microsoft/signalr";

interface WebSocketContextProps {
  playerId: string | null;
  messages: string;
  connect: () => void;
  findGame: () => void;
  leaveGame: () => void;
}

const WebSocketContext = createContext<WebSocketContextProps | undefined>(
  undefined
);

export const WebSocketProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null
  );

  const [playerId, setPlayerId] = useState<string | null>(null);
  const [messages, setMessages] = useState<string>("");

  useEffect(() => {
    const conn = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5000/gamehub")
      .build();

    conn.on("ReceivePlayerId", (id: string) => {
      setPlayerId(id);
      console.log("Player ID:", id);
    });

    setConnection(conn);
  }, []);

  const connect = async () => {
    if (connection) {
      try {
        await connection.start();
        setMessages("Connected to the WebSocket!");
      } catch (err) {
        console.error("Connection error:", err);
        setMessages("Failed to connect.");
      }
    }
  };

  const findGame = async () => {
    if (connection && playerId) {
      await connection.invoke("FindGame", { PlayerId: playerId });
    }
  };

  const leaveGame = async () => {
    if (connection && playerId) {
      await connection.invoke("LeaveGame", { PlayerId: playerId });
    }
  };

  return (
    <WebSocketContext.Provider
      value={{ playerId, messages, connect, findGame, leaveGame }}
    >
      {children}
    </WebSocketContext.Provider>
  );
};

export const useWebSocket = () => {
  const context = useContext(WebSocketContext);
  return context;
};
