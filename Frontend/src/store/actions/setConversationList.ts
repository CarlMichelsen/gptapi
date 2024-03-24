import { conversationOptionMapper } from "../../lib/util/conversationOptionMapper";
import type { ConversationOptionDto } from "../../types/dto/conversation/conversationOption";
import type { ApplicationStore, LoggedInApplicationStore } from "../../types/store/applicationStore";
import type { StoreFunction } from "../storeFunction";

export const setConversationList: StoreFunction<[ConversationOptionDto[]]> = (
    state: ApplicationStore,
    conversationOptionList: ConversationOptionDto[]): ApplicationStore => {
    if (state.state !== "logged-in") return state;

    const dateChunks = conversationOptionMapper(conversationOptionList)
    return {
        ...state,
        conversationChunks: dateChunks,
    } as LoggedInApplicationStore;
}