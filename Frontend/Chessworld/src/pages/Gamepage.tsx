import { useParams } from "react-router-dom";

const Gamepage: React.FC = () => {
  const { gameId } = useParams();

  return (
    <div className="p-6 text-center">
      <h1 className="text-2xl font-bold">Game Room</h1>
      <p>Game ID: {gameId}</p>
      <p>Waiting for the game to start...</p>
    </div>
  );
};

export default Gamepage;
