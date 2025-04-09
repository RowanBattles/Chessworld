import React from "react";
import FindGameComponent from "../components/FindGameComponent";

const Homepage: React.FC = () => {
  return (
    <div className="text-center">
      <h1 className="text-4xl font-bold text-blue-600 my-8">Homepage</h1>
      <FindGameComponent />
    </div>
  );
};

export default Homepage;
