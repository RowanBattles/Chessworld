import React from "react";
import { useParams } from "react-router-dom";

const Game: React.FC = () => {
  const { gameId } = useParams();

  return (
    <div className="p-6 text-center">
      <h1 className="text-2xl font-bold">Game Page</h1>
      <p>Game ID: {gameId || "No Game ID provided"}</p>
    </div>
  );
};

export default Game;
