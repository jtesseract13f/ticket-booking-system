// src/api/nodes.ts
import { apiFetch } from './client';
import type {
    SystemNode,
    NodeInterface,
    AddNodeDto,
    UpdateNodeDto,
} from '../types/node';

export const AVAILABLE_INTERFACES: NodeInterface[] = [
    'CinemaApi',
    'ConcertApi',
    'TicketApi',
];

function jsonBody(payload: unknown): string {
    return JSON.stringify(payload);
}

/** Общий тип для опций запроса — чтобы не дублировать сигнатуру. */
type RequestOptions = {
    signal?: AbortSignal;
    getToken?: () => Promise<string | null> | string | null;
};

/** GET /api/v1/nodes */
export async function fetchNodes(options?: RequestOptions): Promise<SystemNode[]> {
    return apiFetch<SystemNode[]>('/gateway/api/v1/nodes', {
        method: 'GET',
        signal: options?.signal,
        getToken: options?.getToken,
    });
}

/** GET /api/v1/nodes/{id} */
export async function fetchNode(
    id: string,
    options?: RequestOptions,
): Promise<SystemNode | null> {
    try {
        return await apiFetch<SystemNode>(`/gateway/api/v1/nodes/${id}`, {
            method: 'GET',
            signal: options?.signal,
            getToken: options?.getToken,
        });
    } catch (e: any) {
        if (e?.name === 'ApiError' && e.status === 404) return null;
        throw e;
    }
}

/** POST /api/v1/nodes — возвращает id созданного узла. */
export async function addNode(
    dto: AddNodeDto,
    options?: RequestOptions,           // <-- добавили
): Promise<string> {
    return apiFetch<string>('/gateway/api/v1/nodes', {
        method: 'POST',
        body: jsonBody(dto),
        signal: options?.signal,
        getToken: options?.getToken,    // <-- пробрасываем
    });
}

/** PUT /api/v1/nodes */
export async function updateNode(
    id: string,
    patch: Partial<Pick<SystemNode, 'link' | 'status' | 'interface'>>,
    current?: SystemNode,
    options?: RequestOptions,           // <-- добавили
): Promise<string> {
    const body: UpdateNodeDto = {
        id,
        link: patch.link ?? current?.link,
        status: patch.status ?? current?.status,
        interface: patch.interface ?? current?.interface,
    };
    return apiFetch<string>('/gateway/api/v1/nodes', {
        method: 'PUT',
        body: jsonBody(body),
        signal: options?.signal,
        getToken: options?.getToken,
    });
}

/** DELETE /api/v1/nodes/{id} */
export async function deleteNode(
    id: string,
    options?: RequestOptions,           // <-- добавили
): Promise<string> {
    return apiFetch<string>(`/gateway/api/v1/nodes/${id}`, {
        method: 'DELETE',
        signal: options?.signal,
        getToken: options?.getToken,
    });
}