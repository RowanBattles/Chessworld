import React from "react";
import FindGameComponent from "../components/FindGameComponent";

const Homepage: React.FC = () => {
  return (
    <>
      <div className="flex flex-col items-center justify-center min-h-screen text-center">
        <div className="flex flex-col items-center justify-between w-full my-8 px-6 text-center">
          <h1 className="text-4xl font-bold text-blue-600">Homepage</h1>
        </div>
        <div className="w-full p-6 bg-white shadow-md rounded-lg">
          <FindGameComponent />
        </div>
      </div>
    </>
  );
};

export default Homepage;
