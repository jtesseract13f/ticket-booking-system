// src/types/auth.ts
export type Role = 'Admin' | 'User' | 'Guest';

export interface AuthUser {
    id: string;
    role: Role;
    email?: string;
}