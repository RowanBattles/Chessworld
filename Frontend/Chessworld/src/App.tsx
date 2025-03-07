import { Routes, Route } from "react-router-dom";
import Homepage from "./pages/Homepage";
import Gamepage from "./pages/Gamepage";

const App = () => {
  return (
    <Routes>
      <Route path="/" element={<Homepage />} />
      <Route path="/game/:gameId" element={<Gamepage />} />
    </Routes>
  );
};

export default App;
