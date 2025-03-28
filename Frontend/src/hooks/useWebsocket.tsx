// import { useState, useEffect } from "react";
// import * as signalR from "@microsoft/signalr";

// type GameResponse = {
//   role: string;
//   status: string;
// };

// const useWebSocket = (gameId: string) => {
//   const [gameData, setGameData] = useState<GameResponse | null>(null);
//   const [errorStatus, setErrorStatus] = useState<number | null>(null); // Track error status
//   const [loading, setLoading] = useState<boolean>(true); // Track loading state

//   useEffect(() => {
//     const connectWebSocket = async () => {
//       try {
//         const connection = new signalR.HubConnectionBuilder()
//           .withUrl(`/gamehub?gameId=${gameId}`, { withCredentials: true })
//           .withAutomaticReconnect()
//           .build();

//         await connection.start();
//         console.log("Connected to WebSocket");

//         connection.on("GameJoined", (response: GameResponse) => {
//           console.log("GameJoined received:", response);
//           setGameData(response);
//         });

//         setLoading(false); // Connection established, stop loading
//       } catch (err: any) {
//         console.error("WebSocket connection error:", err);

//         // Check if the error is a 404
//         if (err?.message?.includes("404")) {
//           setErrorStatus(404);
//         } else {
//           setErrorStatus(500); // Generic error code for other issues
//         }

//         setLoading(false); // Stop loading after error
//       }
//     };

//     connectWebSocket();

//     return () => {
//       // Cleanup logic if needed
//     };
//   }, [gameId]);

//   return { gameData, errorStatus, loading };
// };

// export default useWebSocket;
