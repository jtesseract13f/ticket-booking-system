import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { AuthProvider } from '@zitadel/react-auth';
import './index.css'
import App from './App.tsx'

const onSigninCallback = (): void => {
    // Очищаем URL от параметров авторизации после входа
    window.history.replaceState({}, document.title, window.location.pathname);
};

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <AuthProvider
            authority={import.meta.env.VITE_ISSUER}
            client_id={import.meta.env.VITE_CLIENT_ID}
            redirect_uri={import.meta.env.VITE_REDIRECT_URI}
            post_logout_redirect_uri={import.meta.env.VITE_POST_LOGOUT_URI}
            scope="openid profile email offline_access"
            onSigninCallback={onSigninCallback}
        >
            <App />
        </AuthProvider>
    </StrictMode>
);