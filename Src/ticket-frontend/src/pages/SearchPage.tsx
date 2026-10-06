// src/pages/SearchPage.tsx
import { useEffect, useState } from 'react';
import { SearchPanel } from '../components/SearchPanel/SearchPanel';
import { TicketList } from '../components/TicketList/TicketList';
import { searchTickets } from '../api/tickets';
import type { Ticket, TicketFilters } from '../types/ticket';

export const SearchPage = () => {
    const [tickets, setTickets] = useState<Ticket[]>([]);
    const [loading, setLoading] = useState(false);
    const [filters, setFilters] = useState<TicketFilters>({});

    useEffect(() => {
        let active = true;
        //setLoading(true);
        searchTickets(filters).then((res) => {
            if (active) {
                setTickets(res);
                setLoading(false);
            }
        });
        return () => { active = false; };
    }, [filters]);

    return (
        <main className="page">
            <SearchPanel onSearch={setFilters} loading={loading} />
            <TicketList tickets={tickets} />
        </main>
    );
};