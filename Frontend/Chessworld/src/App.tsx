import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import { WebSocketProvider } from "./context/WebSocketContext";
import Homepage from "./pages/Homepage";
import Gamepage from "./pages/Gamepage";

const App = () => {
  return (
    <WebSocketProvider>
      <Router>
        <Routes>
          <Route path="/" element={<Homepage />} />
          <Route path="/game/:gameId" element={<Gamepage />} />
        </Routes>
      </Router>
    </WebSocketProvider>
  );
};

export default App;
