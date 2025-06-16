import React from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "./useAuth";

const Navbar: React.FC = () => {
  const navigate = useNavigate();
  const { user } = useAuth();

  console.log("Navbar user:", user);

  return (
    <nav className="w-full bg-white shadow flex items-center justify-between px-6 py-4 border-b border-gray-200">
      <span
        className="text-2xl font-bold text-blue-600 cursor-pointer select-none"
        onClick={() => (window.location.href = "http://localhost:3000")}
      >
        Chessworld
      </span>
      {user ? (
        <span
          className="px-4 py-2 bg-gray-200 text-blue-700 rounded font-semibold cursor-pointer hover:bg-gray-300 transition"
          onClick={() => navigate("/profile")}
          title="Go to profile"
        >
          {user.username}
        </span>
      ) : (
        <button
          className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 transition"
          onClick={() => navigate("/login")}
        >
          Log in
        </button>
      )}
    </nav>
  );
};

export default Navbar;
