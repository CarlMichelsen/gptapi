import type { ReceiveMessage } from "../../types/dto/ReceiveMessage";
import type { ConversationOptionDto } from "../../types/dto/conversationOption";
import type { ApplicationStore } from "../../types/store/applicationStore";
import { applicationStore } from "../applicationStore";
import type { StoreFunction } from "../storeFunction";

export const receiveMessage: StoreFunction<[ReceiveMessage]> = (
    state: ApplicationStore,
    receieveMessage: ReceiveMessage): ApplicationStore => {
    if (state.state !== "logged-in") return state;
    
    if (receieveMessage.conversationId !== null) {
        moveExsistingConversationToTopOfList(state, receieveMessage.conversationId);
    }

    if (state.selectedConversation?.id === receieveMessage.conversationId) {
        

        state.selectedConversation.messages[state.selectedConversation.messages.length-1]?.messageOptions.push(receieveMessage.message);
    }

    return state;
}


const moveExsistingConversationToTopOfList = (state: ApplicationStore, conversationId: string): ApplicationStore => {
    if (state.state !== "logged-in") return state;
    if (state.conversationChunks === null) return state;

    const mostRecentChunk = state.conversationChunks[0] ?? null;

    if (mostRecentChunk !== null) {
        const mostRecentConversation = mostRecentChunk.options[0] ?? null;

        if (mostRecentConversation?.id !== conversationId) {
            const convOption = findConversationOptionById(state, conversationId);
            if (!convOption) return state;

            applicationStore.deleteConversation(conversationId, false);
            applicationStore.addNewConversationToList(convOption);

            return { ...state };
        }
    }

    return state;
}

const findConversationOptionById = (state: ApplicationStore, conversationId: string): ConversationOptionDto|null => {
    if (state.state !== "logged-in") return null;
    const flatConversationOptionList = (state.conversationChunks?.map(c => c.options) ?? []).flat();
    return flatConversationOptionList.find(o => o.id === conversationId) ?? null;
}