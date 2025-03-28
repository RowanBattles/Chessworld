import { Link } from "react-router-dom";

const InternalServerErrorComponent = () => {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen">
      <h1 className="text-3xl font-bold text-red-500">
        500 - Internal Server Error
      </h1>
      <p className="text-gray-700 mt-4">An unexpected error occured.</p>
      <Link
        to="/"
        className="mt-6 px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 transition"
      >
        Return to Homepage
      </Link>
    </div>
  );
};

export default InternalServerErrorComponent;
