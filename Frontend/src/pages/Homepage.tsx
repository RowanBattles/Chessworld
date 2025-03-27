import React from "react";
import FindGameComponent from "../components/FindGameComponent";

const Homepage: React.FC = () => {
  return (
    <>
      <div className="text-4xl font-bold text-center text-blue-600 my-8">
        Homepage
      </div>
      <FindGameComponent />
    </>
  );
};

export default Homepage;
