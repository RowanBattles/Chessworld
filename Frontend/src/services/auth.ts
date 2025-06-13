import axios from "axios";

const API_URL_AUTH = import.meta.env.VITE_API_URL + "/auth";
const API_URL_USER = import.meta.env.VITE_API_URL + "/user";

export const register = async (
  username: string,
  email: string,
  password: string
) => {
  return axios.post(
    `${API_URL_AUTH}/register`,
    {
      userName: username,
      email,
      password,
    },
    { withCredentials: true }
  );
};

export const login = async (email: string, password: string) => {
  return axios.post(
    `${API_URL_AUTH}/login`,
    {
      email,
      password,
    },
    { withCredentials: true }
  );
};

export const logout = async () => {
  await axios.post(`${API_URL_AUTH}/logout`, {}, { withCredentials: true });
};

export const deleteAccount = async () => {
  await axios.delete(`${API_URL_USER}`, { withCredentials: true });
};

export const getCurrentUser = async () => {
  const response = await axios.get(`${API_URL_AUTH}/me`, {
    withCredentials: true,
  });
  return response.data;
};

export const verifyEmail = async (token: string) => {
  const data = await axios.get(`${API_URL_AUTH}/confirm/${token}`);
  console.log("Email verification response:", data);
  return data;
};
