export type SendMessageRequest = {
    messageContent: string;
    conversationId: string | null;
    previousMessageId: string | null;
}