export type ConversationOptionDateChunk = {
    dateText: string;
    options: ConversationOptionDto[];
}

export type ConversationOptionDto = {
    id: string;
    summary: string | null;
    lastAppendedUtc: string;
    createdUtc: string;
};