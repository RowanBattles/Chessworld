import React, { createContext, useContext, useState, useEffect } from "react";
import * as signalR from "@microsoft/signalr";

interface WebSocketContextProps {
  playerId: string | null;
  messages: string;
  connectToMatchmaking: () => void;
}

const WebSocketContext = createContext<WebSocketContextProps | undefined>(
  undefined
);

export const WebSocketProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [matchmakingConnection, setMatchmakingConnection] =
    useState<signalR.HubConnection | null>(null);
  const [playerId, setPlayerId] = useState<string | null>(null);
  const [messages, setMessages] = useState<string>("");

  useEffect(() => {
    const matchmakingConn = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5000/matchmakinghub")
      .configureLogging(signalR.LogLevel.Trace) // Increase logging level
      .build();

    matchmakingConn.on("ReceivePlayerId", (id: string) => {
      setPlayerId(id);
    });

    matchmakingConn.onclose((error) => {
      console.error("Connection closed:", error);
      setMessages("Connection closed.");
    });

    setMatchmakingConnection(matchmakingConn);
  }, []);

  const connectToMatchmaking = async () => {
    if (matchmakingConnection) {
      try {
        await matchmakingConnection.start();
        setMessages("Connected to the Matchmaking WebSocket!");
      } catch (err) {
        console.error("Connection error:", err);
        setMessages("Failed to connect to matchmaking.");
      }
    }
  };

  return (
    <WebSocketContext.Provider
      value={{
        playerId,
        messages,
        connectToMatchmaking,
      }}
    >
      {children}
    </WebSocketContext.Provider>
  );
};

export const useWebSocket = () => {
  const context = useContext(WebSocketContext);
  return context;
};
