type TokenGetter = () => Promise<string | null> | string | null;

let tokenGetter: TokenGetter | null = null;

export function setTokenGetter(getter: TokenGetter | null): void {
    tokenGetter = getter;
}

export async function resolveToken(): Promise<string | null> {
    if (tokenGetter) {
        try {
            const t = await tokenGetter();
            if (t) return t;
        } catch {
            // упавшее получение токена не должно валить запрос
        }
    }
    // fallback — на случай SSR/тестов/если провайдер ещё не смонтирован
    if (typeof localStorage !== 'undefined') {
        return localStorage.getItem('access_token');
    }
    return null;
}