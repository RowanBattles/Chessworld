import React, { createContext, useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  login as apiLogin,
  register as apiRegister,
  logout as apiLogout,
  getCurrentUser,
  deleteAccount as apiDeleteAccount,
} from "../services/auth";
import { UserType } from "../types/UserType";

interface AuthContextType {
  user: UserType | null;
  login: (email: string, password: string) => Promise<void>;
  register: (
    username: string,
    email: string,
    password: string
  ) => Promise<void>;
  logout: () => Promise<void>;
  deleteAccount: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [user, setUser] = useState<UserType | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    getCurrentUser()
      .then((data) => setUser({ username: data.username, email: data.email }))
      .catch(() => setUser(null));
  }, []);

  const login = async (email: string, password: string) => {
    await apiLogin(email, password);
    const data = await getCurrentUser();
    setUser({ username: data.username, email: data.email });
    console.log("User logged in:", data);
    navigate("/profile");
  };

  const register = async (
    username: string,
    email: string,
    password: string
  ) => {
    await apiRegister(username, email, password);
  };

  const logout = async () => {
    await apiLogout();
    setUser(null);
    navigate("/");
  };

  const deleteAccount = async () => {
    await apiDeleteAccount();
    setUser(null);
    navigate("/");
  };

  return (
    <AuthContext.Provider
      value={{ user, login, register, logout, deleteAccount }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};
