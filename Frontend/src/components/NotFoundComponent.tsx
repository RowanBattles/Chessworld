import { Link } from "react-router-dom";

const NotFound = () => {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen">
      <h1 className="text-3xl font-bold text-red-500">404 - PAGE NOT FOUND</h1>
      <p className="text-gray-700 mt-4">
        The page you are looking for does not exist.
      </p>
      <Link
        to="/"
        className="mt-6 px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 transition"
      >
        Return to Homepage
      </Link>
    </div>
  );
};

export default NotFound;
