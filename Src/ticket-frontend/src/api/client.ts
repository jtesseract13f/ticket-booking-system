import { resolveToken } from './tokenProvider';

const API_BASE_URL: string =
    (import.meta as any).env?.VITE_API_BASE_URL ?? 'http://ticket-booking-system.local';

export class ApiError extends Error {
    constructor(public status: number, public body: string) {
        super(`API error ${status}`);
        this.name = 'ApiError';
    }
}

interface FetchOptions extends RequestInit {
    getToken?: () => Promise<string | null> | string | null;
    /** Пропустить авторизацию для публичного эндпоинта. */
    skipAuth?: boolean;
}

export async function apiFetch<T>(path: string, options: FetchOptions = {}): Promise<T> {
    const { getToken, skipAuth, headers: initHeaders, ...rest } = options;

    const token = skipAuth
        ? null
        : (getToken ? await getToken() : await resolveToken());

    const headers = new Headers(initHeaders);
    headers.set('Accept', 'application/json');
    if (rest.body && !headers.has('Content-Type')) {
        headers.set('Content-Type', 'application/json');
    }
    if (token) headers.set('Authorization', `Bearer ${token}`);

    const res = await fetch(`${API_BASE_URL}${path}`, { ...rest, headers });

    if (!res.ok) {
        // 401 — токен просрочен; можно выкинуть событие для logout
        if (res.status === 401 && typeof window !== 'undefined') {
            window.dispatchEvent(new CustomEvent('api:unauthorized'));
        }
        throw new ApiError(res.status, await res.text());
    }
    if (res.status === 204) return undefined as T;
    return (await res.json()) as T;
}