import { playerData } from "./PlayerType";

export type GameType = {
  game: {
    fen: string;
    gameId: string;
    status: string;
  };
  opponent: {
    color: string;
  };
  player: playerData;
};
