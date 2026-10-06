// src/auth/RequireAdmin.tsx
import type { ReactNode } from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '@zitadel/react-auth';
import {useIsAdmin, useRoles} from './roles';
import { useLang } from '../i18n/LangContext';

export const RequireAdmin = ({ children }: { children: ReactNode }) => {
    const { isAuthenticated, isLoading } = useAuth();
    const roles = useRoles();
    const { t } = useLang();

    if (isLoading) return <p className="page">{t.loading}</p>;
    if (!isAuthenticated) return <Navigate to="/" replace />;

    if (!roles.includes('Admin')) {
        return (
            <p className="page">
                {t.noAccess} <br />
                Ваши роли: {roles.length ? roles.join(', ') : '—'}
            </p>
        );
    }

    return <>{children}</>;
};