// src/components/TicketList/TicketList.tsx
import type { Ticket } from '../../types/ticket';
import { TicketCard } from '../TicketCard/TicketCard';

export const TicketList = ({ tickets }: { tickets: Ticket[] }) => {
    if (!tickets.length) {
        return <p className="ticket-list__empty">Ничего не найдено</p>;
    }
    return (
        <div className="ticket-list">
            {tickets.map((t) => <TicketCard key={t.id} ticket={t} />)}
        </div>
    );
};