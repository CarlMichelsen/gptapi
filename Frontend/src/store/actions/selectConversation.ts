import type { ConversationDto } from "../../types/dto/conversation";
import type { ApplicationStore, LoggedInApplicationStore } from "../../types/store/applicationStore";
import type { StoreFunction } from "../storeFunction";

export const selectConversation: StoreFunction<[ConversationDto|null]> = (
    state: ApplicationStore,
    conversation: ConversationDto|null): ApplicationStore => {
    return {
        ...state,
        selectedConversation: conversation,
    } as LoggedInApplicationStore;
}