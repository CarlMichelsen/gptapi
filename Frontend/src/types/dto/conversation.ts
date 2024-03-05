import type { Message } from "./message";

export type ConversationDto = {
    id: string | null;
    summary: string | null;
    messages: MessageContainer[];
};

export type MessageContainer = {
    index: number;
    messageOptions: { [key: string]: Message };
    selectedMessage: string;
};