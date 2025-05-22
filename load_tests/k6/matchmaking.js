import http from "k6/http";
import { sleep } from "k6";

export const options = {
  vus: 1000,
  duration: "20s",
};

export default function () {
  sleep(1);

  let res = http.post("http://localhost:5000/matchmaking/findgame");

  let cookie = res.cookies["playerToken"]?.[0]?.value;

  if (!cookie) {
    console.error("Error: No playerToken cookie found in response.");
    return;
  } else if (cookie.length !== 8) {
    console.error(`Error: Invalid playerToken cookie length: ${cookie.length}`);
    return;
  }
}
