// src/components/AdminHeader/AdminHeader.tsx
import { Link, useLocation } from 'react-router-dom';
import { useLang } from '../../i18n/LangContext';
import { UserMenu } from '../UserMenu/UserMenu';
import { LanguageSwitcher } from '../LanguageSwitcher/LanguageSwitcher';

export const AdminHeader = () => {
    const { t } = useLang();
    const { pathname } = useLocation();

    return (
        <header className="admin-header">
            <div className="admin-header__left">
                <Link to="/" className="admin-header__link">🏠 {t.home}</Link>
                <span className="admin-header__title">{t.admin}</span>
            </div>

            <nav className="admin-header__nav">
                <Link
                    to="/admin"
                    className={`admin-header__link ${pathname === '/admin' ? 'is-active' : ''}`}
                >
                    {t.nodes}
                </Link>
                <Link
                    to="/admin/users"
                    className={`admin-header__link ${pathname === '/admin/users' ? 'is-active' : ''}`}
                >
                    {t.users}
                </Link>

                <LanguageSwitcher variant="dark" />
                <UserMenu />
            </nav>
        </header>
    );
};