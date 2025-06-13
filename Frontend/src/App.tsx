import React from "react";
import { Routes, Route } from "react-router-dom";
import GamePage from "./pages/GamePage";
import Homepage from "./pages/Homepage";
import ErrorPage from "./pages/ErrorPage";
import RegisterPage from "./pages/RegisterPage";
import LoginPage from "./pages/LoginPage";
import ProfilePage from "./pages/ProfilePage";
import CheckYourMailPage from "./pages/CheckYourMailPage";
import VerifyPage from "./pages/VerifyPage";
import Navbar from "./components/Navbar";
const App: React.FC = () => {
  return (
    <>
      <Navbar />
      <Routes>
        <Route path="/" element={<Homepage />} />
        <Route path="/:gameId" element={<GamePage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/profile" element={<ProfilePage />} />
        <Route path="/check-your-mail" element={<CheckYourMailPage />} />
        <Route path="/verify/:token" element={<VerifyPage />} />
        <Route
          path="*"
          element={<ErrorPage statusCode={404} message="Page not found" />}
        />
      </Routes>
    </>
  );
};

export default App;
