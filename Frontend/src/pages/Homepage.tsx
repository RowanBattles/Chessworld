import React from "react";
import { useWebSocket } from "../context/WebSocketContext";

const Homepage: React.FC = () => {
  const webSocketContext = useWebSocket();
  const playerId = webSocketContext?.playerId;
  const opponentId = webSocketContext?.opponentId;
  const messages = webSocketContext?.messages;
  const connect = webSocketContext?.connect;
  const findGame = webSocketContext?.findGame;
  const leaveGame = webSocketContext?.leaveGame;

  return (
    <div className="p-6 text-center">
      <h1 className="text-2xl font-bold">Homepage</h1>
      <p>Player ID: {playerId || "Not connected"}</p>

      <div className="space-x-2 mt-4">
        <button className="px-4 py-2 bg-green-500 text-white" onClick={connect}>
          Connect
        </button>
        <button className="px-4 py-2 bg-blue-500 text-white" onClick={findGame}>
          Find Game
        </button>
        <button className="px-4 py-2 bg-red-500 text-white" onClick={leaveGame}>
          Leave Game
        </button>
      </div>
      {opponentId ? <p>Opponent found: {opponentId}</p> : <p>{messages}</p>}
    </div>
  );
};

export default Homepage;
