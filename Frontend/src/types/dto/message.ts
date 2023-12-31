import type { Role } from "./role";

export type Message = {
    id: string;
    role: Role;
    content: string;
    complete: boolean;
    created: Date;
}