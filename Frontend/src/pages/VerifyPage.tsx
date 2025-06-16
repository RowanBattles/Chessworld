import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { verifyEmail } from "../services/auth";

const VerifyPage: React.FC = () => {
  const { token } = useParams<{ token: string }>();
  const [status, setStatus] = useState<null | number>(null);
  const [message, setMessage] = useState("Verifying...");
  const navigate = useNavigate();

  useEffect(() => {
    if (!token) return;
    verifyEmail(token)
      .then((res) => {
        setStatus(res.status);
        setMessage(res.data);
      })
      .catch((err: { response?: { status?: number; data?: string } }) => {
        setStatus(err.response?.status || 400);
        setMessage(err.response?.data || "Verification failed.");
      });
  }, [token]);

  return (
    <>
      <div className="flex flex-col items-center justify-center min-h-screen bg-gray-100">
        <div className="bg-white p-8 rounded shadow-md w-full max-w-sm text-center">
          {status === null && <p>{message}</p>}
          {status === 200 && (
            <>
              <div className="text-green-500 text-4xl mb-2">✔</div>
              <p className="mb-4">{message}</p>
              <button
                className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 transition"
                onClick={() => navigate("/login")}
              >
                Go to Login
              </button>
            </>
          )}
          {status && status !== 200 && (
            <>
              <div className="text-red-500 text-4xl mb-2">✖</div>
              <p className="mb-4">{message}</p>
              <button
                className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 transition"
                onClick={() => navigate("/register")}
              >
                Go to Register
              </button>
            </>
          )}
        </div>
      </div>
    </>
  );
};

export default VerifyPage;
