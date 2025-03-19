import React from "react";
import { useParams } from "react-router-dom";

const Game: React.FC = () => {
  const { gameId } = useParams();

  return (
    <div>
      <h1>Game Page</h1>
      <p>Game ID: {gameId}</p>
    </div>
  );
};

export default Game;
