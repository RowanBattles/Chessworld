import React from "react";
import FindGameComponent from "../components/FindGameComponent";
import ErrorPage from "./ErrorPage";

const Homepage: React.FC = () => {
  const hasError = false; // Replace with actual error condition if needed

  if (hasError) {
    return <ErrorPage statusCode={500} message="Something went wrong" />;
  }

  return (
    <div className="text-center">
      <h1 className="text-4xl font-bold text-blue-600 my-8">Homepage</h1>
      <FindGameComponent />
    </div>
  );
};

export default Homepage;
