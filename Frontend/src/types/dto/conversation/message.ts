import type { Role } from "./role";

export type Usage = {
    provider: string;
    model: string;
    tokens: number;
}

export type Message = {
    id: string;
    previousMessageId: string|null;
    role: Role;
    content: string;
    complete: boolean;
    createdUtc: Date;
    completedUtc: Date;
    usage: Usage|null;
    visible: boolean;
}