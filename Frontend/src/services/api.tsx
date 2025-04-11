import axios from "axios";

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
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

export const getGameData = async (gameId: string) => {
  const response = await apiClient.get(`/games/${gameId}`);
  return response.data;
};
