import React from "react";
import { useNavigate } from "react-router-dom";

const NotFound: React.FC = () => {
  const navigate = useNavigate();

  const handleGoHome = () => {
    navigate("/");
  };

  return (
    <div className="p-6 text-center">
      <h1 className="text-4xl font-bold text-red-500">404 - Page Not Found</h1>
      <p className="mt-4 text-lg">
        The page you are looking for does not exist.
      </p>
      <button
        onClick={handleGoHome}
        className="mt-6 px-4 py-2 bg-blue-500 text-white font-semibold rounded hover:bg-blue-600"
      >
        Go to Homepage
      </button>
    </div>
  );
};

export default NotFound;
