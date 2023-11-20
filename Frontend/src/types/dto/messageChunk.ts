import type { Message } from "./message";

export type MessageChunk = {
    conversationId: number;
    index: number;
} & Message;