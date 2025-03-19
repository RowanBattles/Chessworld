import React from "react";
import { Routes, Route } from "react-router-dom";
import Homepage from "./pages/Homepage";
import Game from "./pages/Game";

const App: React.FC = () => {
  return (
    <Routes>
      <Route path="/" element={<Homepage />} />
      <Route path="/game/:gameId" element={<Game />} />
    </Routes>
  );
};

export default App;
