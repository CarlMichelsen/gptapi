import type { ChatSection } from ".";

export type CodeSection = {
    type: "code";
    language: string;
} & ChatSection