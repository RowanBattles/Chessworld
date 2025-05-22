import fetch from "node-fetch";

async function validateGames(iterations, failedRequests) {
  let testPassed = true;

  const res = await fetch("http://localhost:5000/games");
  const games = await res.json();

  const seenTokens = new Set();
  const duplicates = [];

  for (const game of games) {
    const { whiteId, blackId, gameId } = game;

    for (const token of [whiteId, blackId]) {
      if (seenTokens.has(token)) {
        duplicates.push({ gameId, token });
      } else {
        seenTokens.add(token);
      }
    }
  }

  let x = iterations - failedRequests;

  if (x % 2 !== 0) {
    x -= 1;
  }

  const correctAmountOfGames = x / 2;

  if (games.length !== correctAmountOfGames) {
    console.error(
      `❌ Expected ${correctAmountOfGames} games, but found ${games.length}.`
    );
    testPassed = false;
  } else {
    console.log(
      `✅ Expected ${correctAmountOfGames} games, and found ${games.length}.`
    );
  }

  function ChanceOnDuplication(X) {
    const differentTokens = 52;
    const tokenAmount = 8;
    const N = differentTokens ** tokenAmount;
    const e = Math.E;
    const exponent = -(X ** 2) / (2 * N);
    const Pduplicate = 1 - e ** exponent;
    return Pduplicate;
  }

  const chance = ChanceOnDuplication(games.length);
  console.log("The chance of duplication is: ", chance.toFixed(11), "%");

  if (duplicates.length === 0) {
    console.log("✅ No duplicate tokens found across games.");
  } else {
    console.warn("❌ Duplicate player tokens detected:");
    console.table(duplicates);
  }

  if (testPassed) {
    console.log("✅ Test passed!");
  } else {
    console.error("❌ Test failed!");
  }
}

const iterations = process.argv[2];
const failedRequests = process.argv[3];
validateGames(iterations, failedRequests);
