import React from "react";
import { Routes, Route } from "react-router-dom";
import GamePage from "./pages/GamePage";
import Homepage from "./pages/Homepage";
import NotFound from "./components/NotFoundComponent"; // Import the NotFound component

const App: React.FC = () => {
  return (
    <Routes>
      <Route path="/" element={<Homepage />} />
      <Route path="/:gameId" element={<GamePage />} />
      <Route path="*" element={<NotFound />} />
    </Routes>
  );
};

export default App;
