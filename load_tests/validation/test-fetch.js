// test-fetch.js
import fetch from "node-fetch";
fetch("http://127.0.0.1:8080/games")
  .then((res) => res.text())
  .then((text) => {
    console.log("Fetch succeeded:", text);
    process.exit(0);
  })
  .catch((err) => {
    console.error("Fetch failed:", err);
    process.exit(1);
  });
