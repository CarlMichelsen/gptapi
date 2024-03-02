import type { Message } from "./message";

export type MessageChunk = {
    conversationId: string;
    index: number;
} & Message;