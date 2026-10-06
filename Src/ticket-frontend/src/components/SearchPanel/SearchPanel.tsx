// src/components/SearchPanel/SearchPanel.tsx
import { useState } from 'react';
import type { EventType, TicketFilters } from '../../types/ticket';

interface Props {
    onSearch: (filters: TicketFilters) => void;
    loading?: boolean;
}

const EVENT_TYPES: { value: EventType; label: string }[] = [
    { value: 'concert', label: 'Концерт' },
    { value: 'movie', label: 'Кино' },
    { value: 'exhibition', label: 'Выставка' },
];

export const SearchPanel = ({ onSearch, loading }: Props) => {
    const [name, setName] = useState('');
    const [datetime, setDatetime] = useState('');
    const [maxPrice, setMaxPrice] = useState('');
    const [eventType, setEventType] = useState<EventType | ''>('');

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        onSearch({
            name: name.trim() || undefined,
            datetime: datetime || undefined,
            maxPrice: maxPrice ? Number(maxPrice) : undefined,
            eventType: eventType || undefined,
        });
    };

    return (
        <form className="search-panel" onSubmit={handleSubmit}>
            <input
                className="search-panel__input"
                type="text"
                placeholder="Название"
                value={name}
                onChange={(e) => setName(e.target.value)}
            />

            <input
                className="search-panel__input"
                type="datetime-local"
                value={datetime}
                onChange={(e) => setDatetime(e.target.value)}
            />

            <input
                className="search-panel__input"
                type="number"
                min="0"
                step="1"
                placeholder="Макс. цена"
                value={maxPrice}
                onChange={(e) => setMaxPrice(e.target.value)}
            />

            <select
                className="search-panel__input"
                value={eventType}
                onChange={(e) => setEventType(e.target.value as EventType | '')}
            >
                <option value="">Все типы</option>
                {EVENT_TYPES.map((t) => (
                    <option key={t.value} value={t.value}>{t.label}</option>
                ))}
            </select>

            <button className="search-panel__submit" type="submit" disabled={loading}>
                {loading ? 'Поиск...' : 'Найти'}
            </button>
        </form>
    );
};