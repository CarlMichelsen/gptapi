import type { ConversationMetadata, ConversationType } from "../dto/conversation"
import type { OAuthUser } from "../dto/oAuthUser"

type LoggedOutApplicationStore = {
    user: null
}

type LoggedInApplicationStore = {
    user: OAuthUser
    selectedConversation: ConversationType | null;
    conversations: ConversationMetadata[] | null;
}

export type ApplicationStore = LoggedOutApplicationStore | LoggedInApplicationStore;