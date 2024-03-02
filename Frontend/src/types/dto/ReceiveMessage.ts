import type { Message } from "./message";

export type ReceiveMessage = {
    conversationId: string | null;
    message: Message;
};