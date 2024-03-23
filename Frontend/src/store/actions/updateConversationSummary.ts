import type { ApplicationStore } from "../../types/store/applicationStore";
import type { StoreFunction } from "../storeFunction";

export const updateConversationSummary: StoreFunction<[string, string]> = (
    state: ApplicationStore,
    conversationId: string,
    summary: string): ApplicationStore => {
    if (state.state !== "logged-in") return state;
    
    if (state.selectedConversation?.id === conversationId) {
        state.selectedConversation.summary = summary;
    }

    const conversationOptionList = state.conversationChunks?.map(s => s.options).flat() ?? [];
    const found = conversationOptionList.find(c => c.id === conversationId);
    if (found) {
        found.summary = summary;
    }

    return { ...state };
}