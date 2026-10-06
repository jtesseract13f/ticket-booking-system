// src/components/UserMenu/UserMenu.tsx
import { useAuth } from '@zitadel/react-auth';

export const UserMenu = () => {
    const { user, signoutPopup } = useAuth();

    const name = user?.profile?.name || user?.profile?.email || 'Профиль';

    return (
        <div className="user-menu">
            <span className="user-menu__name">{name}</span>
            <button
                type="button"
                className="user-menu__logout"
                onClick={() => signoutPopup()}
            >
                Выйти
            </button>
        </div>
    );
};