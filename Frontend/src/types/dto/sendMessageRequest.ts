export type SendMessageRequest = {
    temporaryUserMessageId: string;
    messageContent: string;
    conversationId: string | null;
    previousMessageId: string | null;
}