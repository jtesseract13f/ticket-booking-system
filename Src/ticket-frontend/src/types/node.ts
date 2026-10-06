// src/types/node.ts
export type NodeStatus = 'on' | 'off';

export type NodeInterface = 'CinemaApi' | 'ConcertApi' | 'TicketApi';

export interface SystemNode {
    id: string;               // GUID
    link: string;
    status: NodeStatus;
    interface: NodeInterface;
}

/** DTO'шки для API — подгони названия полей под C#-модели */
export interface AddNodeDto {
    link: string;
    status: NodeStatus;
    interface: NodeInterface;
}

export interface UpdateNodeDto {
    id: string;
    link?: string;
    status?: NodeStatus;
    interface?: NodeInterface;
}