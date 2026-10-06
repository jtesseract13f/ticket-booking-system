// src/pages/TicketDetailPage.tsx
import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { getTicketById } from '../api/tickets';
import type { Ticket } from '../types/ticket';
import { PLACEHOLDER_IMAGE } from '../mocks/tickets';

const EVENT_TYPE_LABEL: Record<Ticket['eventType'], string> = {
    concert: 'Концерт',
    movie: 'Кино',
    exhibition: 'Выставка',
    other: 'Другое',
};

export const TicketDetailPage = () => {
    const { id } = useParams<{ id: string }>();
    const [ticket, setTicket] = useState<Ticket | null | undefined>(undefined);

    useEffect(() => {
        if (!id) return;
        getTicketById(id).then(setTicket);
    }, [id]);

    if (ticket === undefined) return <p className="page">Загрузка...</p>;
    if (ticket === null) return <p className="page">Мероприятие не найдено</p>;

    return (
        <main className="page detail">
            <Link to="/" className="detail__back">← Назад к поиску</Link>

            <img
                className="detail__image"
                src={ticket.imageUrl || PLACEHOLDER_IMAGE}
                alt={ticket.name}
            />

            <h1 className="detail__title">{ticket.name}</h1>
            <p className="detail__type">{EVENT_TYPE_LABEL[ticket.eventType]}</p>

            <dl className="detail__list">
                <dt>Геолокация</dt><dd>{ticket.location}</dd>
                <dt>Дата и время (UTC)</dt>
                <dd>{new Date(ticket.datetime).toISOString()}</dd>
                <dt>Минимальная стоимость</dt>
                <dd>{ticket.minPrice === 0 ? 'Бесплатно' : `${ticket.minPrice} ₽`}</dd>
                <dt>Идентификатор</dt><dd>{ticket.id}</dd>
                <dt>Организатор</dt><dd>{ticket.organizer}</dd>
                <dt>Ссылка</dt>
                <dd>
                    <a href={ticket.link} target="_blank" rel="noreferrer noopener">
                        {ticket.link}
                    </a>
                </dd>
                {ticket.description && (
                    <>
                        <dt>Описание</dt><dd>{ticket.description}</dd>
                    </>
                )}
            </dl>
        </main>
    );
};