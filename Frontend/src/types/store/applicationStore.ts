import type { ConversationMetadata, ConversationType } from "../dto/conversation"
import type { OAuthUser } from "../dto/oAuthUser"

export type LoggedOutApplicationStore = {
    state: "logged-out"
}

export type LoggedInApplicationStore = {
    state: "logged-in"
    user: OAuthUser
    selectedConversation: ConversationType | null;
    conversations: ConversationMetadata[] | null;
}

export type ApplicationStore = LoggedOutApplicationStore | LoggedInApplicationStore;