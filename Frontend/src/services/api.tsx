import axios from "axios";

const API_BASE_URL = "http://localhost:5000/matchmaking";

export const findGame = async () => {
  const response = await axios.post(`${API_BASE_URL}/findgame`);
  return response.data;
};

export const getMatchStatus = async (playerId: string) => {
  const response = await axios.get(`${API_BASE_URL}/matchstatus/${playerId}`);
  return response.data;
};
