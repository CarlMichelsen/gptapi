import { writable } from 'svelte/store';
import type { ApplicationStore } from '../types/store/applicationStore';
import { deleteCookie, getUserData } from '../clients/userDataClient';
import type { ConversationDto } from '../types/dto/conversation';
import { getQueryParams, setQueryParam } from '../util/queryParameters';
import type { Message } from '../types/dto/message';
import type { ReceiveMessage } from '../types/dto/ReceiveMessage';
import { ConversationClient } from '../clients/conversationClient';

// Function to create a custom store
const createApplicationStore = (initialValue: ApplicationStore) => {
    // Create writable
    const store = writable<ApplicationStore>(initialValue);
    const conversationQueryParameterName = "c";
    const conversationClient = new ConversationClient();

    const login = async () => {
        const oauthUser = await getUserData();
        if (oauthUser) {
            const conversationListResponse = await conversationClient.getConversationList();
            if (!conversationListResponse.ok) {
                console.error("conversationListResponse not ok", ...conversationListResponse.errors);
                return;
            }

            store.update((value) => ({ ...value, user: oauthUser, conversationChunks: conversationListResponse.data, state: "logged-in", ready: true } as ApplicationStore));
            const queryConversationId = getQueryParams()[conversationQueryParameterName] ?? null;
            selectConversation(queryConversationId);
        } else {
            store.update((value) => ({ ...value, ready: true } as ApplicationStore));
        }
    }

    const logout = async () => {
        await deleteCookie();
        store.set({ state: "logged-out", ready: true });
    }

    const selectConversation = async (conversationId: string | null) => {
        let conv: ConversationDto | null = null;
        if (conversationId) {
            const response = await conversationClient.getConversation(conversationId);
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

    const deleteConversation = async (conversationId: string) => {
        const deleted = await conversationClient.deleteConversation(conversationId);
        if (!deleted) return;

        store.update((value) => {
            if (value.state !== "logged-in") return value;
            
            const dateChunk = value.conversationChunks?.find(dc => !!dc.options.find(c => c.id === conversationId)) ?? null;
            if (!dateChunk) return value;
            
            const idx = dateChunk.options.findIndex(c => c.id === conversationId);
            if (idx === -1) return value;
            dateChunk.options.splice(idx, 1);

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
            const convMetadata = value.conversationChunks?.map(o => o.options).flat().find(o => o.id === conversationId) ?? null;
            if (convMetadata !== null) {
                convMetadata.summary = summary;
                return { ...value };
            }
            return value;
        });
    }

    const receieveMessage = async (receiveMessageObj: ReceiveMessage) => {
        store.update((value) => {
            if (value.state == "logged-out") return value;
            if (!value.selectedConversation) return value;
            
            const prevMsgCon = receiveMessageObj.message.previousMessageId
                ? value.selectedConversation.messages.find(msgCon => !!msgCon.messageOptions[receiveMessageObj.message.previousMessageId!]) ?? null
                : null;
            
            if (prevMsgCon == null) {
                const map: { [key: string]: Message } = {};
                map[receiveMessageObj.message.id] = receiveMessageObj.message;

                const currentMsgCon = {
                    index: 0,
                    messageOptions: map,
                    selectedMessage: receiveMessageObj.message.id,
                };
                value.selectedConversation.messages.push(currentMsgCon);
            } else {
                const exsisting = value.selectedConversation.messages.find(msgCon => msgCon.index === prevMsgCon.index+1)
                if (exsisting)
                {
                    const currentMsgCon = exsisting;
                    currentMsgCon.messageOptions[receiveMessageObj.message.id] = receiveMessageObj.message;
                    currentMsgCon.selectedMessage = receiveMessageObj.message.id;

                    console.log("A message was edited, hurray!", receiveMessageObj.message.content);
                } else {
                    const map: { [key: string]: Message } = {};
                    map[receiveMessageObj.message.id] = receiveMessageObj.message;

                    const currentMsgCon = {
                        index: prevMsgCon.index + 1,
                        messageOptions: map,
                        selectedMessage: receiveMessageObj.message.id,
                    }
                    value.selectedConversation.messages.push(currentMsgCon);
                }
            }

            return { ...value };
        });
    }

    return {
        subscribe: store.subscribe,
        login,
        logout,
        selectConversation,
        deleteConversation,
        updateConversationSummary,
        receieveMessage
    };
}

// Initial state
const initialState: ApplicationStore = {
    ready: false,
    state: "logged-out",
};

// Create the store with initial value
export const applicationStore = createApplicationStore(initialState);
