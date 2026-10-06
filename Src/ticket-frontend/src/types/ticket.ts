// src/types/ticket.ts
export type EventType = 'concert' | 'movie' | 'exhibition' | 'other';

export interface Ticket {
  id: string;               // GUID
  name: string;             // до 256
  location: string;         // до 256
  datetime: string;         // ISO UTC
  minPrice: number;         // вещественное
  description?: string;     // до 4096
  organizer: string;        // до 256
  link: string;             // до 256
  imageUrl?: string;
}

export interface TicketFilters {
  name?: string;
  datetime?: string;
  maxPrice?: number;
  eventType?: EventType | '';
}