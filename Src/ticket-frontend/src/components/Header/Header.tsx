// src/components/Header/Header.tsx
import { Link } from 'react-router-dom';
import { useAuth } from '@zitadel/react-auth';
import { UserMenu } from '../UserMenu/UserMenu';
import { LanguageSwitcher } from '../LanguageSwitcher/LanguageSwitcher';
import { useIsAdmin } from '../../auth/roles';
import { useLang } from '../../i18n/LangContext';

export const Header = () => {
    const { isAuthenticated, signinPopup } = useAuth();
    const isAdmin = useIsAdmin();
    const { t } = useLang();

    return (
        <header className="header">
            <Link to="/" className="header__logo">🎟 Билеты</Link>

            <nav className="header__nav">
                <Link to="/">{t.search}</Link>

                {isAdmin && (
                    <Link to="/admin" className="header__admin-link">
                        {t.admin}
                    </Link>
                )}

                <LanguageSwitcher variant="light" />

                {isAuthenticated ? (
                    <UserMenu />
                ) : (
                    <button type="button" onClick={() => signinPopup()}>{t.login}</button>
                )}
            </nav>
        </header>
    );
};