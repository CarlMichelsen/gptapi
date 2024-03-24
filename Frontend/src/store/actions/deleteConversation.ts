import type { ApplicationStore } from "../../types/store/applicationStore";
import type { StoreFunction } from "../storeFunction";

export const deleteConversation: StoreFunction<[string, boolean?]> = (
    state: ApplicationStore,
    conversationId: string,
    unselectConversationIfNeeded: boolean = true) => {
    if (state.state === "logged-out") return state;
    
    if (state.conversationChunks !== null) {
        const relevantChunk = state.conversationChunks.find(c => !!c.options.find(c => c.id === conversationId)) ?? null;

        if (relevantChunk !== null) {
            relevantChunk.options = relevantChunk.options.filter(item => item.id !== conversationId);

            if (relevantChunk.options.length === 0) {
                state.conversationChunks = state.conversationChunks.filter(c => c.options.length !== 0);
            }
        }
    }

    let selected = state.selectedConversation;
    if (unselectConversationIfNeeded) {
        selected = state.selectedConversation?.id === conversationId
            ? null
            : state.selectedConversation;
    }

    return {
        ...state,
        selectedConversation: selected,
    };
}