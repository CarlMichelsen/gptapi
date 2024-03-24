import type { ConversationDto } from "../../types/dto/conversation/conversation";
import type { ApplicationStore } from "../../types/store/applicationStore";
import type { StoreFunction } from "../storeFunction";

export const selectConversation: StoreFunction<[ConversationDto|null]> = (
    state: ApplicationStore,
    conversation: ConversationDto|null): ApplicationStore => {
    if (state.state !== "logged-in") return state;

    return {
        ...state,
        selectedConversation: conversation,
    };
}