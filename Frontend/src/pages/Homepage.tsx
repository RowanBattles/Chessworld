import React from "react";
import { useWebSocket } from "../context/WebSocketContext";

const Homepage: React.FC = () => {
  const webSocketContext = useWebSocket();
  const playerId = webSocketContext?.playerId;
  const messages = webSocketContext?.messages;
  const connectToMatchmaking = webSocketContext?.connectToMatchmaking;

  return (
    <div className="p-6 text-center">
      <h1 className="text-2xl font-bold">Homepage</h1>
      <p>Player ID: {playerId || "Not connected"}</p>

      <div className="space-x-2 mt-4">
        <button
          className="px-4 py-2 bg-green-500 text-white"
          onClick={connectToMatchmaking}
        >
          Connect
        </button>
      </div>
      <p>{messages}</p>
    </div>
  );
};

export default Homepage;
