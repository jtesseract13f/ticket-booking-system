// src/components/NodeRow/NodeRow.tsx
import type { NodeInterface, NodeStatus, SystemNode } from '../../types/node';
import { AVAILABLE_INTERFACES } from '../../api/nodes';
import { useLang } from '../../i18n/LangContext';

interface Props {
    node: SystemNode;
    onChange: (
        id: string,
        patch: Partial<Pick<SystemNode, 'status' | 'interface'>>,
    ) => void;
    onDelete?: (id: string) => void;
}

const NodeRow = ({ node, onChange, onDelete }: Props) => {  // ← добавили onDelete
    const { t } = useLang();

    return (
        <tr className="node-row">
            <td className="node-row__link">
                <button
                    type="button"
                    className="node-row__delete"
                    onClick={() => onDelete?.(node.id)}
                    aria-label="Delete node"
                >
                    ×
                </button>
                <a href={node.link} target="_blank" rel="noreferrer noopener">
                    {node.link}
                </a>
            </td>

            <td>
                <label className="switch">
                    <input
                        type="checkbox"
                        checked={node.status === 'on'}
                        onChange={(e) =>
                            onChange(node.id, {
                                status: (e.target.checked ? 'on' : 'off') as NodeStatus,
                            })
                        }
                    />
                    <span className="switch__slider" />
                    <span className="switch__label">
                        {node.status === 'on' ? t.on : t.off}
                    </span>
                </label>
            </td>

            <td>
                <select
                    className="node-row__select"
                    value={node.interface}
                    onChange={(e) =>
                        onChange(node.id, { interface: e.target.value as NodeInterface })
                    }
                >
                    {AVAILABLE_INTERFACES.map((i) => (
                        <option key={i} value={i}>
                            {t.interfaceNames[i]}
                        </option>
                    ))}
                </select>
            </td>
        </tr>
    );
};

export default NodeRow;