export enum DateRange {
    today,
    yesterday,
    week,
    month,
    year,
    unknown,
}

export type ConversationOptionDateChunk = {
    dateRange: DateRange;
    options: ConversationOptionDto[];
}

export type ConversationOptionDto = {
    id: string;
    summary: string | null;
    lastAppendedUtc: string;
    createdUtc: string;
};