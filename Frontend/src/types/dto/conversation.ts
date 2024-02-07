import type { Message } from "./message";

export type ConversationType = {
    id: string | null;
    summary: string | null;
    messages: MessageContainer[];
};

export type MessageContainer = {
    index: number;
    messageOptions: Map<string, Message>;
    selectedMessage: string;
};

export type ConversationMetadata = {
    id: string;
    summary: string | null;
    lastAppendedUtc: Date;
    createdUtc: Date;
};