// src/mocks/tickets.ts
import type { Ticket } from '../types/ticket';

export const PLACEHOLDER_IMAGE = '/Игла.jpg';

export const mockTickets: Ticket[] = [
    {
        id: '11111111-1111-1111-1111-111111111111',
        name: 'Рок-фестиваль «Лето»',
        location: 'Москва, Лужники',
        datetime: '2026-06-15T18:00:00Z',
        minPrice: 2500.0,
        description: 'Большой летний фестиваль с участием известных групп.',
        organizer: 'Live Nation Russia',
        link: 'https://example.com/rock-fest',
        imageUrl: PLACEHOLDER_IMAGE,
    },
    {
        id: '22222222-2222-2222-2222-222222222222',
        name: 'Премьера фильма «Космос»',
        location: 'Санкт-Петербург, Кинотеатр «Аврора»',
        datetime: '2026-05-20T19:30:00Z',
        minPrice: 700.0,
        organizer: 'Кинопрокат СПб',
        link: 'https://example.com/space-premiere',
        imageUrl: PLACEHOLDER_IMAGE,
    },
    {
        id: '33333333-3333-3333-3333-333333333333',
        name: 'Выставка «Импрессионисты»',
        location: 'Москва, Пушкинский музей',
        datetime: '2026-04-10T10:00:00Z',
        minPrice: 1200.0,
        description: 'Более 100 работ французских импрессионистов.',
        organizer: 'Пушкинский музей',
        link: 'https://example.com/impressionists',
        imageUrl: PLACEHOLDER_IMAGE,
    },
    {
        id: '44444444-4444-4444-4444-444444444444',
        name: 'Стендап-концерт',
        location: 'Казань, Дом актёра',
        datetime: '2026-07-01T20:00:00Z',
        minPrice: 1500.0,
        organizer: 'Comedy Club',
        link: 'https://example.com/standup',
        imageUrl: PLACEHOLDER_IMAGE,
    },
    {
        id: '55555555-5555-5555-5555-555555555555',
        name: 'Научный фестиваль «Future»',
        location: 'Новосибирск, Экспоцентр',
        datetime: '2026-09-12T09:00:00Z',
        minPrice: 0.0,
        description: 'Лекции, мастер-классы и демонстрации технологий.',
        organizer: 'Science Hub',
        link: 'https://example.com/future',
        imageUrl: PLACEHOLDER_IMAGE,
    },
];