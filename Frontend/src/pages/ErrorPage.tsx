import React from "react";
import { useNavigate } from "react-router-dom";

interface ErrorPageProps {
  statusCode: number;
  message: string;
}

const ErrorPage: React.FC<ErrorPageProps> = ({ statusCode, message }) => {
  const navigate = useNavigate();

  const handleGoHome = () => {
    navigate("/");
  };

  return (
    <div className="p-6 text-center">
      <h1 className="text-4xl font-bold text-red-500">
        {statusCode} - {message}
      </h1>
      <p className="mt-4 text-lg">
        {statusCode === 404
          ? "The page you are looking for does not exist."
          : "An error occurred. Please try again later."}
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

export default ErrorPage;
