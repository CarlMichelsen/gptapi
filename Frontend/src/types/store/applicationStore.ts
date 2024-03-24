import type { ConversationDto } from "../dto/conversation/conversation"
import type { ConversationOptionDateChunk } from "../dto/conversation/conversationOption"
import type { OAuthUser } from "../dto/oAuthUser"

export type LoggedOutApplicationStore = {
    ready: boolean;
    state: "logged-out";
}

export type LoggedInApplicationStore = {
    ready: true;
    state: "logged-in";
    user: OAuthUser;
    selectedConversation: ConversationDto | null;
    conversationChunks: ConversationOptionDateChunk[] | null;
}

export type ApplicationStore = LoggedOutApplicationStore | LoggedInApplicationStore;