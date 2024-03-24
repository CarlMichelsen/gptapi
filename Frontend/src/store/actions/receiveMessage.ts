import type { ReceiveMessage } from "../../types/dto/conversation/ReceiveMessage";
import type { MessageContainer } from "../../types/dto/conversation/conversation";
import { DateRange, type ConversationOptionDto, type ConversationOptionDateChunk } from "../../types/dto/conversation/conversationOption";
import type { Message } from "../../types/dto/conversation/message";
import type { ApplicationStore, LoggedInApplicationStore } from "../../types/store/applicationStore";
import type { StoreFunction } from "../storeFunction";

export const receiveMessage: StoreFunction<[ReceiveMessage]> = (
    state: ApplicationStore,
    receieveMessage: ReceiveMessage): ApplicationStore => {
    if (state.state !== "logged-in") return state;
    
    moveconversationToTopOfList(state, receieveMessage.conversationId);

    if (state.selectedConversation?.id === receieveMessage.conversationId) {
        const message = receieveMessage.message;
        const convMsgs = state.selectedConversation.messages;
        
        appendMessageToConversationAfterRelevantMessage(message, convMsgs);
    }

    return { ...state };
}

const appendMessageToConversationAfterRelevantMessage = (message: Message, convMsgs: MessageContainer[]) => {
    if (message.previousMessageId) {
        const relevantContainer = convMsgs.find(cm => cm.messageOptions[message.previousMessageId!]) ?? null;

        if (relevantContainer) {
            const followingContainer = convMsgs.find(c => c.index === relevantContainer.index+1);
            if (followingContainer) {
                followingContainer.messageOptions[message.id] = message;
            } else {
                const newFollowingContainer: MessageContainer = {
                    index: relevantContainer.index + 1,
                    messageOptions: { [message.id]: message },
                    selectedMessage: message.id,
                };

                convMsgs.push(newFollowingContainer);
            }
        } else {
            const lastContainer = convMsgs.toSorted((a, b) => a.index - b.index)[0] ?? null;

            const newFollowingContainer: MessageContainer = {
                index: lastContainer ? lastContainer.index + 1 : 0,
                messageOptions: { [message.id]: message },
                selectedMessage: message.id,
            };

            convMsgs.push(newFollowingContainer);
        }
    }
}


const moveconversationToTopOfList = (state: ApplicationStore, conversationId: string) => {
    if (state.state !== "logged-in") return;
    if (state.conversationChunks === null) return;

    const convOption = findConversationOptionById(state, conversationId);
    if (!convOption) {
        const nowString = (new Date()).toUTCString();

        const newConvOption: ConversationOptionDto = {
            id: conversationId,
            summary: "New conversation",
            lastAppendedUtc: nowString,
            createdUtc: nowString,
        };

        addConversationOptionToTopOfList(state, newConvOption);
        return;
    }
    
    // Delete exsisting conversation from list
    if (state.conversationChunks !== null) {
        const relevantChunk = state.conversationChunks.find(c => !!c.options.find(c => c.id === conversationId)) ?? null;

        if (relevantChunk !== null) {
            relevantChunk.options = relevantChunk.options.filter(item => item.id !== conversationId);

            if (relevantChunk.options.length === 0) {
                state.conversationChunks = state.conversationChunks.filter(c => c.options.length !== 0);
            }
        }
    }

    // Re-add conversation to top of list
    convOption.lastAppendedUtc = (new Date()).toUTCString();
    addConversationOptionToTopOfList(state, convOption);
}

const addConversationOptionToTopOfList = (state: LoggedInApplicationStore, convOption: ConversationOptionDto)  => {
    if (state.conversationChunks === null) return;
    
    let todayChunk = state.conversationChunks.find(c => c.dateRange === DateRange.today) ?? null;
    if (!todayChunk) {
        todayChunk = {
            dateRange: DateRange.today,
            options: [convOption],
        } as ConversationOptionDateChunk;
        state.conversationChunks.unshift(todayChunk);
    } else {
        todayChunk.options.unshift(convOption);
    }
}

const findConversationOptionById = (state: ApplicationStore, conversationId: string): ConversationOptionDto|null => {
    if (state.state !== "logged-in") return null;
    const flatConversationOptionList = (state.conversationChunks?.map(c => c.options) ?? []).flat();
    return flatConversationOptionList.find(o => o.id === conversationId) ?? null;
}