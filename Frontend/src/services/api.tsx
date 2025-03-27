import axios from "axios";

const apiClient = axios.create({
  baseURL: "http://localhost:5000",
  withCredentials: true,
});

export const findGame = async () => {
  const response = await apiClient.post("/matchmaking/findgame", {});
  return response.data;
};

export const getMatchStatus = async () => {
  const response = await apiClient.get("/matchmaking/matchstatus");
  return response.data;
};
