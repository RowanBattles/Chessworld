import { useEffect, useState } from "react";
import { Chessboard } from "react-chessboard";
import useWebSocket from "../hooks/useWebsocket";

const parseTurnFromFen = (fen: string): "white" | "black" => {
  const turnChar = fen.split(" ")[1];
  return turnChar === "w" ? "white" : "black";
};

const ChessBoard = ({
  color,
  isSpectator,
  fen,
  gameId,
  playerData,
}: {
  color: "white" | "black";
  isSpectator: boolean;
  fen: string;
  gameId: string;
  playerData: { id: string; isSpectator: boolean };
}) => {
  const [currentFen, setCurrentFen] = useState(fen);
  const [currentTurn, setCurrentTurn] = useState<"white" | "black">(
    parseTurnFromFen(fen)
  );
  const { sendMove, onReceiveMove, offReceiveMove } = useWebSocket(
    gameId,
    playerData
  );

  useEffect(() => {
    const handleReceiveMove = (fen: string) => {
      setCurrentFen(fen);
      setCurrentTurn(parseTurnFromFen(fen));
    };

    onReceiveMove(handleReceiveMove);

    return () => {
      offReceiveMove(handleReceiveMove);
    };
  }, [onReceiveMove, offReceiveMove]);

  const isPlayerTurn = color === currentTurn;

  const onDrop = (sourceSquare: string, targetSquare: string) => {
    if (isSpectator) {
      console.error("Spectators cannot make moves!");
      return false;
    }

    if (!isPlayerTurn) {
      console.error("It's not your turn!");
      return false;
    }

    const move = `${sourceSquare}${targetSquare}`;
    sendMove(move);
    return true;
  };

  const isDraggablePiece = ({ piece }: { piece: string }): boolean => {
    if (isSpectator) return false;
    if (!isPlayerTurn) return false;
    const isWhitePiece = piece.startsWith("w");
    const isBlackPiece = piece.startsWith("b");

    return (
      (color === "white" && isWhitePiece) || (color === "black" && isBlackPiece)
    );
  };

  return (
    <div className="flex justify-center">
      <Chessboard
        id="BasicBoard"
        position={currentFen}
        onPieceDrop={onDrop}
        boardOrientation={color}
        isDraggablePiece={isDraggablePiece}
        customBoardStyle={{
          borderRadius: "8px",
          boxShadow: "0 5px 15px rgba(0,0,0,0.3)",
        }}
      />
    </div>
  );
};

export default ChessBoard;
