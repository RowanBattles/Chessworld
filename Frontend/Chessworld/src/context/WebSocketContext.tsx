import React, { createContext, useContext, useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
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
  const navigate = useNavigate(); // Navigation hook

  useEffect(() => {
    const conn = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5000/gamehub")
      .build();

    conn.on("ReceivePlayerId", (id: string) => {
      setPlayerId(id);
      console.log("Player ID:", id);
    });

    conn.on("GameFound", (gameId: string) => {
      console.log("Game found! ID:", gameId);
      setMessages(`Game found! Redirecting to game ${gameId}...`);
      navigate(`/game/${gameId}`); // Redirect to game page
    });

    conn.on("WaitingForOpponent", () => {
      setMessages("Waiting for an opponent...");
    });

    conn.on("GameLeft", (message: string) => {
      setMessages(message);
    });

    setConnection(conn);
  }, [navigate]);

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
      try {
        await connection.invoke("FindGame", { PlayerId: playerId });
      } catch (err) {
        console.error("FindGame error:", err);
        setMessages("Failed to find game.");
      }
    }
  };

  const leaveGame = async () => {
    if (connection && playerId) {
      try {
        await connection.invoke("LeaveGame", { PlayerId: playerId });
      } catch (err) {
        console.error("LeaveGame error:", err);
        setMessages("Failed to leave game.");
      }
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
