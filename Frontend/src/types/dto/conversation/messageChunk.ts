import type { Message } from "./message";

export type MessageChunk = {
    conversationId: string;
    streamIdentifier: string;
    index: number;
} & Message;