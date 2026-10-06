import { apiFetch } from './client';

/** Соответствует C# Filter на шлюзе (query-параметры). */
export interface EventsFilter {
    eventType?: string;
    price?: number;
    dateFrom?: string; // ISO-8601, желательно UTC
    dateTo?: string;
}

/** Соответствует C# EventDto (System.Text.Json камелизует PascalCase). */
export interface EventDto {
    externalId: string;
    cost: number | null;
    place: string | null;
    name: string | null;
    description: string | null;
    startDate: string | null;  // ISO-8601
    uri: string | null;
}

function buildQuery(filter: EventsFilter): string {
    const params = new URLSearchParams();
    if (filter.eventType) params.set('eventType', filter.eventType);
    if (filter.price != null) params.set('price', String(filter.price));
    if (filter.dateFrom) params.set('dateFrom', filter.dateFrom);
    if (filter.dateTo) params.set('dateTo', filter.dateTo);
    const qs = params.toString();
    return qs ? `?${qs}` : '';
}

export function searchEvents(
    filter: EventsFilter,
    options?: { getToken?: () => string | null; signal?: AbortSignal },
): Promise<EventDto[]> {
    return apiFetch<EventDto[]>(`/gateway/api/v1/events${buildQuery(filter)}`, {
        method: 'GET',
        signal: options?.signal,
        getToken: options?.getToken,
    });
}