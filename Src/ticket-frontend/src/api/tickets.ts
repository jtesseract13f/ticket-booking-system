import type { Ticket, TicketFilters } from '../types/ticket';
import { searchEvents, type EventDto } from './events';

/**
 * Маппинг DTO шлюза → UI-модель.
 * Поля, которых нет в EventDto (organizer, imageUrl), заполняем безопасными значениями.
 */
function mapEventToTicket(e: EventDto): Ticket {
    return {
        id: e.externalId,
        name: e.name ?? 'Без названия',
        location: e.place ?? 'Место не указано',
        datetime: e.startDate ?? new Date().toISOString(),
        minPrice: e.cost ?? 0,
        description: e.description ?? undefined,
        organizer: '',           // TODO: добавить в EventDto на бэке, если нужно
        link: e.uri ?? '',
        imageUrl: undefined,     // TODO: аналогично
    };
}

export async function searchTickets(
    filters: TicketFilters,
    options?: { getToken?: () => string | null; signal?: AbortSignal },
): Promise<Ticket[]> {
    const events = await searchEvents(
        {
            eventType: filters.eventType || undefined,
            price: filters.maxPrice ?? undefined,
            dateFrom: filters.datetime || undefined,
            // dateTo в TicketFilters пока нет — при необходимости добавь поле
        },
        options,
    );

    return events.map(mapEventToTicket);
}

export async function getTicketById(id: string): Promise<Ticket | undefined> {
    // Отдельного GET /events/{id} на шлюзе пока нет — тянем список и ищем.
    // Когда появится эндпоинт — замени на прямой запрос.
    const tickets = await searchTickets({});
    return tickets.find((t) => t.id === id);
}