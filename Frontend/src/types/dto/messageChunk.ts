import type { Message } from "./message";

export type MessageChunk = {
    index: number;
    created: Date;
} & Message;