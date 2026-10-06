import { useEffect } from 'react';
import { useAuth } from '@zitadel/react-auth';
import { setTokenGetter } from '../api/tokenProvider';

export function AuthTokenBridge({ children }: { children?: React.ReactNode }) {
    const { user, isAuthenticated } = useAuth();

    useEffect(() => {
        if (!isAuthenticated || !user) {
            setTokenGetter(null);
            return;
        }

        setTokenGetter(() => {
            // user.access_token — стандартное поле oidc-client-ts User
            const token = (user as { access_token?: string }).access_token;
            return token ?? null;
        });
    }, [isAuthenticated, user]);

    return <>{children}</>;
}