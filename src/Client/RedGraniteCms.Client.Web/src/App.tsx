import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { AddPageForm } from './features/pages/addPage/addPageForm';
import { EditPageForm } from "./features/pages/editPage/editPageForm";
import { HomePage } from "./pages/HomePage";
import { ErrorBoundary } from "./components";

function App() {
  return (
    <ErrorBoundary>
      <div className="w-full h-full flex flex-col items-center">
        <h1 className="text-3xl font-bold mt-6 mb-2">RedGraniteCms</h1>
        <hr className="w-full border-border mb-4" />
        <Router>
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/addPage" element={<AddPageForm />} />
            <Route path="/editPage/:pageId" element={<EditPageForm />} />
          </Routes>
        </Router>
      </div>
    </ErrorBoundary>
  );
}

export default App;
