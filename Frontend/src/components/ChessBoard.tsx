import { useEffect, useState, useRef } from "react";
import { Chess } from "chess.js";
import { Chessboard } from "react-chessboard";
import { HubConnection } from "@microsoft/signalr";

const ChessBoard = ({
  color,
  isSpectator,
  fen,
  socket,
}: {
  color: "white" | "black";
  isSpectator: boolean; // Add isSpectator as a required prop
  fen: string;
  socket: HubConnection | null;
}) => {
  const [game] = useState(new Chess(fen));
  const [currentFen, setCurrentFen] = useState(fen);
  const [isPlayerTurn, setIsPlayerTurn] = useState(() => {
    const turn = fen.split(" ")[1];
    return (
      (turn === "w" && color === "white") || (turn === "b" && color === "black")
    );
  });
  const boardRef = useRef<any>(null);

  useEffect(() => {
    if (!socket) return;

    const handleReceiveMove = (move: any) => {
      console.log("Move received from backend:", move);
      game.move(move);
      setCurrentFen(game.fen());

      const nextTurn = game.turn();
      setIsPlayerTurn(
        (nextTurn === "w" && color === "white") ||
          (nextTurn === "b" && color === "black")
      );
    };

    socket.on("ReceiveMove", handleReceiveMove);

    return () => {
      socket.off("ReceiveMove", handleReceiveMove);
    };
  }, [socket, game, color]);

  const onDrop = (sourceSquare: string, targetSquare: string) => {
    if (isSpectator) {
      console.error("Spectators cannot make moves!");
      return false;
    }

    if (!isPlayerTurn) {
      console.error("It's not your turn!");
      return false;
    }

    const move = {
      from: sourceSquare,
      to: targetSquare,
      promotion: "q",
    };

    const result = game.move(move);
    if (result) {
      setCurrentFen(game.fen());
      setIsPlayerTurn(false);

      if (socket?.state === "Connected") {
        const uciMove = `${move.from}${move.to}${move.promotion || ""}`;
        console.log("Sending move to backend:", uciMove);

        socket
          .invoke("MakeMove", uciMove)
          .then(() => {
            console.log("Move sent to backend:", uciMove);
          })
          .catch((error) => {
            console.error("Failed to send move to backend:", error);
          });
      } else {
        console.error("WebSocket connection is not established.");
      }
    }
    return !!result;
  };

  const isDraggablePiece = ({ piece }: { piece: string }): boolean => {
    if (isSpectator) return false; // Disable dragging for spectators
    if (!isPlayerTurn) return false; // Disable dragging if it's not the player's turn

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
        ref={boardRef}
      />
    </div>
  );
};

export default ChessBoard;
