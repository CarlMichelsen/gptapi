import type { ConversationMetadata, ConversationType } from "../dto/conversation"
import type { SteamPlayer } from "../dto/steamPlayer"

type LoggedOutApplicationStore = {
    user: null
}

type LoggedInApplicationStore = {
    user: SteamPlayer
    selectedConversation: ConversationType | null;
    conversations: ConversationMetadata[] | null;
}

export type ApplicationStore = LoggedOutApplicationStore | LoggedInApplicationStore;