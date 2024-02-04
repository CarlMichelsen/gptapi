import { writable } from 'svelte/store';
import type { ApplicationStore } from '../types/store/applicationStore';
import { deleteCookie, getUserData } from '../clients/userDataClient';
import { getConversation, getConversationList, deleteConversation as deleteConversationClientFunction } from '../clients/conversationClient';
import type { ConversationMetadata, ConversationType } from '../types/dto/conversation';
import { getQueryParams, setQueryParam } from '../util/queryParameters';
import type { Message } from '../types/dto/message';
import type { UpdateMessageId } from '../types/dto/updateMessageId';

// Function to create a custom store
const createApplicationStore = (initialValue: ApplicationStore) => {
    // Create writable
    const store = writable<ApplicationStore>(initialValue);
    const conversationQueryParameterName = "c";

    const login = async () => {
        const oauthUser = await getUserData();
        if (oauthUser) {
            const conversationListResponse = await getConversationList();
            if (!conversationListResponse.ok) {
                console.error("conversationListResponse not ok", ...conversationListResponse.errors);
                return;
            }

            store.update((value) => ({ ...value, user: oauthUser, conversations: conversationListResponse.data, state: "logged-in" } as ApplicationStore));
            const queryConversationId = getQueryParams()[conversationQueryParameterName] ?? null;
            if (queryConversationId) selectConversation(queryConversationId);
        }
    }

    const logout = async () => {
        await deleteCookie();
        store.set({ state: "logged-out" });
    }

    const selectConversation = async (conversationId: string | null) => {
        let conv: ConversationType | null = null;
        if (conversationId) {
            const response = await getConversation(conversationId);
            if (response.ok) {
                conv = response.data;
            }
        }
        
        store.update((value)  => {
            if (value.state !== "logged-in") return value;
            return { ...value, selectedConversation: conv } as ApplicationStore;
        });
        setQueryParam(conversationQueryParameterName, conv?.id);
    }

    const addNewConversation = async (conversationId: string) => {
        store.update((value) => {
            if (value.state  !== "logged-in") return value;
            const conv: ConversationMetadata = {
                id: conversationId,
                summary: null,
                createdUtc: new Date(),
                lastAppendedUtc: new Date(),
            }

            value.conversations = [ conv, ...(value.conversations ?? []) ];
            return value;
        });

        await selectConversation(conversationId);
    }

    const updateMessageId = (updateMessageId: UpdateMessageId) => {
        store.update((value) => {
            if (value.state  !== "logged-in") return value;
            if (updateMessageId.conversationId !== value.selectedConversation?.id) return value;
            
            const msg = value.selectedConversation.messages.find(m => m.id === updateMessageId.temporaryUserMessageId);
            if (msg) {
                msg.id = updateMessageId.replacementUserMessageId;
            }

            return value;
        });
    }

    const deleteConversation = async (conversationId: string) => {
        const deleted = await deleteConversationClientFunction(conversationId);
        if (!deleted) return;

        store.update((value) => {
            if (value.state !== "logged-in") return value;
            const conversationListId = value.conversations?.findIndex(c => c.id === conversationId) ?? -1;

            if (conversationListId === -1) return value;
            value.conversations!.splice(conversationListId, 1);

            let res = { ...value };
            if (conversationId === value.selectedConversation?.id) {
                res = { ...res, selectedConversation: null };
                setQueryParam(conversationQueryParameterName, null);
            }
            
            return res;
        });
    }

    const updateConversationSummary = (conversationId: string, summary: string) => {
        store.update((value) => {
            if (value.state !== "logged-in") return value;
            const convMetadata = value.conversations?.find(o => o.id === conversationId) ?? null;
            if (convMetadata !== null) {
                convMetadata.summary = summary;
                return { ...value };
            }
            return value;
        });
    }

    const receieveMessage = (message: Message) => {
        store.update((value) => {
            if (value.state == "logged-out") return value;
            if (!value.selectedConversation) return value;

            value.selectedConversation.messages.push(message);
            return { ...value };
        });
    }

    return {
        subscribe: store.subscribe,
        login,
        logout,
        selectConversation,
        updateMessageId,
        addNewConversation,
        deleteConversation,
        updateConversationSummary,
        receieveMessage
    };
}

// Initial state
const initialState: ApplicationStore = {
    state: "logged-out",
};

// Create the store with initial value
export const applicationStore = createApplicationStore(initialState);
