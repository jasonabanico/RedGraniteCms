import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import { AddPageForm } from './features/pages/addPage/addPageForm';
import { EditPageForm } from "./features/pages/editPage/editPageForm";
import { HomePage } from "./pages/HomePage";
import { LoginPage } from "./pages/LoginPage";
import { ErrorBoundary } from "./components";
import { isAuthenticated, logout, getEmail } from "./services/auth";

function RequireAuth({ children }: { children: React.ReactNode }) {
  return isAuthenticated() ? <>{children}</> : <Navigate to="/login" replace />;
}

function App() {
  return (
    <ErrorBoundary>
      <div className="w-full h-full flex flex-col items-center">
        <div className="w-full flex items-center justify-between px-4 mt-6 mb-2">
          <h1 className="text-3xl font-bold">RedGraniteCms</h1>
          {isAuthenticated() && (
            <div className="flex items-center gap-3 text-sm">
              <span className="text-muted-foreground">{getEmail()}</span>
              <button className="underline text-sm" onClick={() => { logout(); window.location.reload(); }}>
                Logout
              </button>
            </div>
          )}
        </div>
        <hr className="w-full border-border mb-4" />
        <Router basename="/admin">
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/" element={<RequireAuth><HomePage /></RequireAuth>} />
            <Route path="/addPage" element={<RequireAuth><AddPageForm /></RequireAuth>} />
            <Route path="/editPage/:pageId" element={<RequireAuth><EditPageForm /></RequireAuth>} />
          </Routes>
        </Router>
      </div>
    </ErrorBoundary>
  );
}

export default App;
