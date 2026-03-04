import styled from "styled-components";
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { AddPageForm } from './features/pages/addPage/addPageForm';
import { EditPageForm } from "./features/pages/editPage/editPageForm";
import { HomePage } from "./pages/HomePage";
import { ErrorBoundary } from "./components";

const AppContainer = styled.div`
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
`;

function App() {
  return (
    <ErrorBoundary>
      <AppContainer>
        <h1>RedGraniteCms</h1>
        <hr />
        <Router>
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/addPage" element={<AddPageForm />} />
            <Route path="/editPage/:pageId" element={<EditPageForm />} />
          </Routes>
        </Router>
      </AppContainer>
    </ErrorBoundary>
  );
}

export default App;
