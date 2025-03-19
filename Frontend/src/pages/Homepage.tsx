import React, { useState } from "react";
import FindGameButton from "../components/FindGameButton";

const Homepage: React.FC = () => {
  const [message, setMessage] = useState<string | null>(null);

  return (
    <div className="p-6 text-center">
      <h1 className="text-2xl font-bold">Homepage</h1>
      <p>Welcome to Chessworld!</p>

      <div className="space-x-2 mt-4">
        <FindGameButton setMessage={setMessage} />
      </div>

      {message && <p className="text-blue-500 mt-4">{message}</p>}
    </div>
  );
};

export default Homepage;
