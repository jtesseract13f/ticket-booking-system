import { useAuth } from '@zitadel/react-auth';
import './App.css';

function App() {

  const auth = useAuth();
  console.log(Object.keys(auth)); // увидите все доступные свойства и методы
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
      <div>
        <h1>Привет, {user?.profile?.name || user?.profile?.email}!</h1>
        <button onClick={() => signoutPopup()}>Выйти</button>
        {/* Здесь ваш основной контент для авторизованных пользователей */}
        <p>Вы успешно вошли в систему.</p>
      </div>
  );
}


export default App;