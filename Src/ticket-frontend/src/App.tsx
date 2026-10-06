import { useAuth } from '@zitadel/react-auth';
import { AuthTokenBridge } from './auth/AuthTokenBridge';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { Header } from './components/Header/Header';
import { Footer } from './components/Footer/Footer';
import { SearchPage } from './pages/SearchPage';
import { TicketDetailPage } from './pages/TicketDetailPage';
import { AdminNodesPage } from './pages/admin/AdminNodesPage';
import { AdminUsersPage } from './pages/admin/AdminUsersPage';
import { LangProvider } from './i18n/LangContext';
import { RequireAdmin } from './auth/RequireAdmin';
import {useRoles} from "./auth/roles.ts";
import './App.css';

function App() {

  const auth = useAuth();
  const { user, isAuthenticated, isLoading, signinPopup, signoutPopup } = useAuth();
if (isLoading) {
    return <div>Загрузка...</div>;
  }

  if (!isAuthenticated) {
    return (
        <div>
          <h1>Добро пожаловать!</h1>
          <button onClick={() => signinPopup()}>Войти через Identity Provider</button>
        </div>
    );
  }

  return (
      <LangProvider>
          <AuthTokenBridge>
          <BrowserRouter>
              <Routes>
                  {/* Публичная часть — со своим хэдером/футером */}
                  <Route
                      path="/*"
                      element={
                          <div className="app">
                              <Header />
                              <Routes>
                                 <Route path="/" element={<SearchPage />} />
                                  <Route path="/ticket/:id" element={<TicketDetailPage />} />
                              </Routes>
                              <Footer />
                          </div>
                      }
                  />

                  {/* Админка — со своим AdminHeader */}
                  <Route
                      path="/admin"
                      element={
                          <RequireAdmin>
                              <AdminNodesPage />
                          </RequireAdmin>
                      }
                  />
                  <Route
                      path="/admin/users"
                      element={
                          <RequireAdmin>
                              <AdminUsersPage />
                          </RequireAdmin>
                      }
                  />
              </Routes>
          </BrowserRouter>
          </AuthTokenBridge>
      </LangProvider>
  );
}


export default App;