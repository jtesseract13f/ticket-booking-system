// src/pages/admin/AdminUsersPage.tsx
import { AdminHeader } from '../../components/AdminHeader/AdminHeader';
import {useLang} from '../../i18n/LangContext';

export const AdminUsersPage = () => {
    const { t } = useLang();
    return (
        <div className="admin-layout">
            <AdminHeader />
            <main className="page">
                <h1 className="page__title">{t.users}</h1>
                <p>🚧 {t.loading /* подставь свой текст-заглушку */}</p>
            </main>
        </div>
    );
};