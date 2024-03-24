import type { PromptSetting } from "./promptSetting";

export type SendMessageRequest = {
    messageContent: string;
    conversationId: string | null;
    previousMessageId: string | null;
    promptSetting: PromptSetting;
}