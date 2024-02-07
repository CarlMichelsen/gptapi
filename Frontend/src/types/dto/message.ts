import type { Role } from "./role";

export type Message = {
    id: string;
    previousMessageId: string|null;
    role: Role;
    content: string;
    complete: boolean;
    created: Date;
}