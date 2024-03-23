import { conversationOptionMapper } from "../../lib/util/conversationOptionMapper";
import type { ConversationOptionDto } from "../../types/dto/conversationOption";
import type { ApplicationStore } from "../../types/store/applicationStore";
import type { StoreFunction } from "../storeFunction";

export const addNewConversationToList: StoreFunction<[ConversationOptionDto]> = (
    state: ApplicationStore,
    conversationOption: ConversationOptionDto): ApplicationStore => {
    if (state.state !== "logged-in") return state;

    const conversationList = state.conversationChunks?.map(c => c.options).flat() ?? [];
    conversationList.unshift(conversationOption);

    return {
        ...state,
        conversationChunks: conversationOptionMapper(conversationList) };
}