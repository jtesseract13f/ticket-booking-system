// src/auth/roles.ts
import { useAuth } from '@zitadel/react-auth';

/** Достаёт роли из объекта user, где бы они ни лежали. */
export function useRoles(): string[] {
    const { user } = useAuth();
    if (!user) return [];

    const roles = new Set<string>();
    const u = user as any;
    const p = u.profile ?? {};

    // 1) Прямой claim role — строка или массив (твой случай)
    for (const src of [u, p]) {
        const r = src?.role;
        if (Array.isArray(r)) r.forEach((x: string) => roles.add(x));
        else if (typeof r === 'string') roles.add(r);
    }

    // 2) roles / groups / permissions — на случай другой конфигурации IdP
    for (const src of [u, p]) {
        for (const key of ['roles', 'groups', 'permissions'] as const) {
            const v = src?.[key];
            if (Array.isArray(v)) v.forEach((x: string) => roles.add(x));
            else if (typeof v === 'string') roles.add(v);
        }
    }

    // 3) Zitadel project roles: { "Admin": { "orgId": "orgName" } }
    const projectRoles =
        u['urn:zitadel:iam:org:project:roles'] ??
        p['urn:zitadel:iam:org:project:roles'];

    if (projectRoles && typeof projectRoles === 'object') {
        Object.keys(projectRoles).forEach((r) => roles.add(r));
    }

    return [...roles];
}

export function useIsAdmin(): boolean {
    return useRoles().includes('Admin');
}