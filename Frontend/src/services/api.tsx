import axios from "axios";

const API_BASE_URL = "http://localhost:5000";

export const findGame = async () => {
  const response = await axios.post(`${API_BASE_URL}/matchmaking/findgame`);
  return response.data;
};

export const getMatchStatus = async () => {
  const response = await axios.get(`${API_BASE_URL}/matchmaking/matchstatus`, {
    withCredentials: true,
  });
  console.log("hit", response.data);
  return response.data;
};
