import React from "react";
import { Routes, Route } from "react-router-dom";
import GamePage from "./pages/GamePage";
import Homepage from "./pages/Homepage";
import ErrorPage from "./pages/ErrorPage";

const App: React.FC = () => {
  return (
    <Routes>
      <Route path="/" element={<Homepage />} />
      <Route path="/:gameId" element={<GamePage />} />
      <Route
        path="*"
        element={<ErrorPage statusCode={404} message="Page not found" />}
      />
    </Routes>
  );
};

export default App;
