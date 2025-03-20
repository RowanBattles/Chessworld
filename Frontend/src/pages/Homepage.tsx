import React, { useState } from "react";
import FindGameButton from "../components/FindGameButton";

const Homepage: React.FC = () => {
  const [message, setMessage] = useState<string | null>(null);
  const [isError, setIsError] = useState(false);

  return (
    <div className="p-6 text-center">
      <h1 className="text-2xl font-bold">Homepage</h1>
      <p>Welcome to Chessworld!</p>

      <div className="space-x-2 mt-4">
        <FindGameButton setMessage={setMessage} setIsError={setIsError} />
      </div>

      {message && (
        <p className={`mt-4 ${isError ? "text-red-500" : "text-blue-500"}`}>
          {message}
        </p>
      )}
    </div>
  );
};

export default Homepage;
