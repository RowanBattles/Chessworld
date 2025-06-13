import React from "react";
import { useNavigate } from "react-router-dom";

const CheckYourMailPage: React.FC = () => {
  const navigate = useNavigate();

  return (
    <>
      <div className="flex items-center justify-center min-h-screen bg-gray-100">
        <div className="bg-white p-8 rounded shadow-md w-full max-w-sm text-center">
          <h2 className="text-2xl font-bold mb-4 text-green-600">
            Check your mail
          </h2>
          <p className="text-gray-700 mb-6">
            We’ve sent you an email with further instructions. Please check your
            inbox.
          </p>
          <div className="flex justify-center space-x-4">
            <span
              className="text-blue-600 cursor-pointer hover:underline"
              onClick={() => navigate("/")}
            >
              Homepage
            </span>
            <span className="text-gray-400">|</span>
            <span
              className="text-blue-600 cursor-pointer hover:underline"
              onClick={() => navigate("/login")}
            >
              Login
            </span>
          </div>
        </div>
      </div>
    </>
  );
};

export default CheckYourMailPage;
