// src/components/TicketCard/TicketCard.tsx
import { Link } from 'react-router-dom';
import type { Ticket } from '../../types/ticket';
import { PLACEHOLDER_IMAGE } from '../../mocks/tickets';

const EVENT_TYPE_LABEL: Record<Ticket['eventType'], string> = {
    concert: 'Концерт',
    movie: 'Кино',
    exhibition: 'Выставка',
    other: 'Другое',
};

export const TicketCard = ({ ticket }: { ticket: Ticket }) => (
    <Link to={`/ticket/${ticket.id}`} className="ticket-card">
        <img
            className="ticket-card__image"
            src={ticket.imageUrl || PLACEHOLDER_IMAGE}
            alt={ticket.name}
        />
        <div className="ticket-card__body">
            <h3 className="ticket-card__title">{ticket.name}</h3>
            <p className="ticket-card__meta">{EVENT_TYPE_LABEL[ticket.eventType]}</p>
            <p className="ticket-card__meta">{ticket.location}</p>
            <p className="ticket-card__meta">
                {new Date(ticket.datetime).toLocaleString('ru-RU', { timeZone: 'UTC' })}
            </p>
            <p className="ticket-card__price">
                {ticket.minPrice === 0 ? 'Бесплатно' : `от ${ticket.minPrice} ₽`}
            </p>
        </div>
    </Link>
);