import type { Message } from "./message";

export type ConversationType = {
    id: string | null;
    summary: string | null;
    messages: Message[];
};

export type ConversationMetadata = {
    id: string;
    summary: string | null;
    lastAppended: Date;
    created: Date;
};