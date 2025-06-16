import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../components/useAuth";

const ProfilePage: React.FC = () => {
  const { user, logout, deleteAccount } = useAuth();
  const [showModal, setShowModal] = useState(false);
  const [deleteInput, setDeleteInput] = useState("");
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    if (!user) {
      navigate("/register");
    }
  }, [user, navigate]);

  const handleLogout = async () => {
    await logout();
  };

  const handleDeleteAccount = () => {
    setShowModal(true);
    setDeleteInput("");
    setDeleteError(null);
  };

  const confirmDelete = async () => {
    if (deleteInput !== "DELETE") {
      setDeleteError('You must type "DELETE" to confirm.');
      return;
    }
    try {
      await deleteAccount();
      setShowModal(false);
    } catch (err) {
      setDeleteError(
        err instanceof Error ? err.message : "An unexpected error occurred"
      );
    }
  };

  const handleModify = () => {
    window.alert("Modify profile is not implemented yet.");
  };

  if (!user) return null;

  return (
    <>
      <div className="flex items-center justify-center min-h-screen bg-gray-100">
        <div className="bg-white p-8 rounded shadow-md w-full max-w-sm text-center">
          <h1 className="text-2xl font-bold mb-4 text-blue-600">Profile</h1>
          <p className="mb-2 text-gray-700">Username: {user.username}</p>
          <p className="mb-6 text-gray-700">Email: {user.email}</p>
          <div className="flex flex-col space-y-3">
            <button
              className="w-full bg-blue-500 text-white py-2 rounded hover:bg-gray-500 transition"
              onClick={handleModify}
            >
              Modify
            </button>
            <button
              className="w-full bg-red-600 text-white py-2 rounded hover:bg-red-600 transition"
              onClick={handleLogout}
            >
              Logout
            </button>
            <button
              className="w-full bg-red-700 text-white py-2 rounded hover:bg-red-800 transition"
              onClick={handleDeleteAccount}
            >
              Delete Account
            </button>
          </div>
        </div>
      </div>
      {showModal && (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-40 z-50">
          <div className="bg-white p-6 rounded shadow-lg w-full max-w-xs text-center">
            <h2 className="text-xl font-bold mb-2 text-red-600">
              Are you sure?
            </h2>
            <p className="mb-4 text-gray-700">
              This action cannot be undone.
              <br />
              Type <span className="font-mono font-bold">DELETE</span> to
              confirm.
            </p>
            <input
              type="text"
              value={deleteInput}
              onChange={(e) => setDeleteInput(e.target.value)}
              className="w-full px-3 py-2 border rounded mb-2"
              placeholder="Type DELETE"
            />
            {deleteError && (
              <p className="text-red-500 text-sm mb-2">{deleteError}</p>
            )}
            <div className="flex justify-between space-x-2">
              <button
                className="flex-1 bg-gray-300 text-gray-700 py-2 rounded hover:bg-gray-400 transition"
                onClick={() => setShowModal(false)}
              >
                Cancel
              </button>
              <button
                className="flex-1 bg-red-700 text-white py-2 rounded hover:bg-red-800 transition"
                onClick={confirmDelete}
              >
                Delete
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default ProfilePage;
