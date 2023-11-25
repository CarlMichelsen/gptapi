export type SectionType = "text"|"code";

export type ChatSection = {
    type: SectionType;
    content: string;
}