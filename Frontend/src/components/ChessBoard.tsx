import { useEffect, useState } from "react";
import { Chessboard } from "react-chessboard";
import { HubConnection } from "@microsoft/signalr";

const parseTurnFromFen = (fen: string): "white" | "black" => {
  const turnChar = fen.split(" ")[1];
  return turnChar === "w" ? "white" : "black";
};

const ChessBoard = ({
  color,
  isSpectator,
  fen,
  socket,
}: {
  color: "white" | "black";
  isSpectator: boolean;
  fen: string;
  socket: HubConnection | null;
}) => {
  const [currentFen, setCurrentFen] = useState(fen);
  const [currentTurn, setCurrentTurn] = useState<"white" | "black">(
    parseTurnFromFen(fen)
  );

  useEffect(() => {
    if (!socket) return;

    const handleReceiveMove = (fen: string) => {
      setCurrentFen(fen);
      setCurrentTurn(parseTurnFromFen(fen));
    };

    socket.on("ReceiveMove", handleReceiveMove);

    return () => {
      socket.off("ReceiveMove", handleReceiveMove);
    };
  }, [socket, color]);

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

    if (socket?.state === "Connected") {
      console.log("Sending move to backend:", move);

      socket.invoke("MakeMove", move).catch((error) => {
        console.error("Failed to send move to backend:", error);
      });
    } else {
      console.error("WebSocket connection is not established.");
      return false;
    }

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
