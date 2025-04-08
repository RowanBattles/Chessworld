import { useEffect, useState } from "react";
import { Chessboard } from "react-chessboard";
import { HubConnection } from "@microsoft/signalr";

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
  const [isPlayerTurn, setIsPlayerTurn] = useState(() => {
    const turn = fen.split(" ")[1]; // Extract turn from FEN ("w" or "b")
    return (
      (turn === "w" && color === "white") || (turn === "b" && color === "black")
    );
  });

  useEffect(() => {
    const turn = fen.split(" ")[1]; // Extract turn from FEN
    setIsPlayerTurn(
      (turn === "w" && color === "white") || (turn === "b" && color === "black")
    );
    setCurrentFen(fen); // Update the board with the latest FEN
  }, [fen, color]);

  useEffect(() => {
    if (!socket) return;

    const handleReceiveMove = (fen: string) => {
      console.log("Received FEN from server:", fen);

      // Update the board state
      setCurrentFen(fen);

      // Update the turn based on the new FEN
      const nextTurn = fen.split(" ")[1]; // Extract turn from FEN
      setIsPlayerTurn(
        (nextTurn === "w" && color === "white") ||
          (nextTurn === "b" && color === "black")
      );
    };

    socket.on("ReceiveMove", handleReceiveMove);

    return () => {
      socket.off("ReceiveMove", handleReceiveMove);
    };
  }, [socket, color]);

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
