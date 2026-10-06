// src/pages/admin/AdminNodesPage.tsx
import { useEffect, useState } from 'react';
import { AdminHeader } from '../../components/AdminHeader/AdminHeader';
import NodeRow from '../../components/NodeRow/NodeRow';
import {
    addNode,
    deleteNode,
    fetchNodes,
    updateNode,
    AVAILABLE_INTERFACES,
} from '../../api/nodes';
import type { AddNodeDto, NodeInterface, SystemNode } from '../../types/node';
import { useLang } from '../../i18n/LangContext';

export const AdminNodesPage = () => {
    const { t } = useLang();
    const [nodes, setNodes] = useState<SystemNode[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [draftLink, setDraftLink] = useState('');
    const [draftInterface, setDraftInterface] = useState<NodeInterface>(
        AVAILABLE_INTERFACES[0],
    );
    const [creating, setCreating] = useState(false);

    // ─────────────── загрузка ───────────────
    useEffect(() => {
        const ctrl = new AbortController();
        //setLoading(true);
        fetchNodes({ signal: ctrl.signal })
            .then((data) => {
                setNodes(data);
                setError(null);
            })
            .catch((e) => {
                if (e?.name !== 'AbortError') setError(String(e?.message ?? e));
            })
            .finally(() => setLoading(false));
        return () => ctrl.abort();
    }, []);

    // ─────────────── обновление ───────────────
    const handleChange = async (
        id: string,
        patch: Partial<Pick<SystemNode, 'status' | 'interface'>>,
    ) => {
        const current = nodes.find((n) => n.id === id);
        setNodes((prev) => prev.map((n) => (n.id === id ? { ...n, ...patch } : n)));
        try {
            await updateNode(id, patch, current);
            setNodes(await fetchNodes()); // ответ — только id, перечитываем
        } catch (err: any) {
            setError(String(err?.message ?? err));
            setNodes(await fetchNodes()); // откат
        }
    };

    // ─────────────── добавление ───────────────
    const handleAdd = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!draftLink.trim() || creating) return;

        const dto: AddNodeDto = {
            link: draftLink.trim(),
            interface: draftInterface,
            // status: 'on',  // если обязателен в AddNodeDto — раскомментируй
        };

        setCreating(true);
        try {
            await addNode(dto);
            setDraftLink('');
            setDraftInterface(AVAILABLE_INTERFACES[0]);
            setNodes(await fetchNodes());
            setError(null);
        } catch (err: any) {
            setError(String(err?.message ?? err));
        } finally {
            setCreating(false);
        }
    };

    // ─────────────── удаление ───────────────
    const handleDelete = async (id: string) => {
        if (!window.confirm('Удалить узел?')) return;

        const snapshot = nodes;
        setNodes((prev) => prev.filter((n) => n.id !== id));
        try {
            await deleteNode(id);
        } catch (err: any) {
            setNodes(snapshot);
            setError(String(err?.message ?? err));
        }
    };

    return (
        <div className="admin-layout">
            <AdminHeader />
            <main className="page">
                <h1 className="page__title">{t.nodes}</h1>

                <form className="node-form" onSubmit={handleAdd}>
                    <input
                        type="text"
                        placeholder={t.link}
                        value={draftLink}
                        onChange={(e) => setDraftLink(e.target.value)}
                        required
                    />
                    <select
                        value={draftInterface}
                        onChange={(e) =>
                            setDraftInterface(e.target.value as NodeInterface)
                        }
                    >
                        {AVAILABLE_INTERFACES.map((iface) => (
                            <option key={iface} value={iface}>
                                {iface}
                            </option>
                        ))}
                    </select>
                    <button type="submit" disabled={creating}>
                        {creating ? t.loading : 'Добавить'}
                    </button>
                </form>

                {error && <p className="page__error">{error}</p>}

                {loading ? (
                    <p>{t.loading}</p>
                ) : (
                    <div className="node-table-wrap">
                        <table className="node-table">
                            <thead>
                            <tr>
                                <th>{t.link}</th>
                                <th>{t.status}</th>
                                <th>{t.interface}</th>
                                <th />
                            </tr>
                            </thead>
                            <tbody>
                            {nodes.map((n) => (
                                <NodeRow
                                    key={n.id}
                                    node={n}
                                    onChange={handleChange}
                                    onDelete={handleDelete}
                                />
                            ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </main>
        </div>
    );
};