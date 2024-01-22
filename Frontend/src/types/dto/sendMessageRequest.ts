export type SendMessageRequest = {
    conversationId: string | null;
    messageContent: string;
    previousMessageId: string | null;
}