// src/mocks/nodes.ts
import type { SystemNode } from '../types/node';

export const mockNodes: SystemNode[] = [
    {
        id: 'aaaaaaaa-0000-0000-0000-000000000001',
        link: 'https://auth.internal/api',
        status: 'on',
        interface: 'CinemaApi',
    },
    {
        id: 'aaaaaaaa-0000-0000-0000-000000000002',
        link: 'https://tickets.internal/api',
        status: 'on',
        interface: 'ConcertApi',
    },
    {
        id: 'aaaaaaaa-0000-0000-0000-000000000003',
        link: 'https://search.internal/graphql',
        status: 'off',
        interface: 'TicketApi',
    },
    {
        id: 'aaaaaaaa-0000-0000-0000-000000000004',
        link: 'wss://events.internal/stream',
        status: 'on',
        interface: 'CinemaApi',
    },
    {
        id: 'aaaaaaaa-0000-0000-0000-000000000005',
        link: 'https://payments.internal/api',
        status: 'off',
        interface: 'TicketApi',
    },
];