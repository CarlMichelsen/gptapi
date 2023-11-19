import { writable } from 'svelte/store';
import type { ConversationStore } from '../types/store/conversationStore';
import type { ConversationMetadata } from '../types/dto/conversation';

// Initial state
const initialState: ConversationStore = {
    conversationOptions: null,
};

// Function to create a custom store
const createConversationStore = (initialValue: ConversationStore) => {
    // Create writable
    const store = writable<ConversationStore>(initialValue);

    const assignOptions = (options: ConversationMetadata[]) => {
        store.set({ conversationOptions: options });
    }

    const addOption = (option: ConversationMetadata) => {
        store.update((value) => {
            if (value.conversationOptions) {
                value.conversationOptions = [ option, ...value.conversationOptions ];
                return { ...value };
            }

            return value;
        });
    }

    const unAssignOptions = () => {
        store.set({ conversationOptions: null });
    }

    const updateSummary = (conversationId: string, summary: string) => {
        store.update((value) => {
            let convMetadata = value.conversationOptions?.find(o => o.id === conversationId) ?? null;
            if (convMetadata !== null) {
                convMetadata.summary = summary;
                return { ...value };
            }
            return value;
        });
    }

    return {
        subscribe: store.subscribe,
        assignOptions,
        addOption,
        unAssignOptions,
        updateSummary
    };
}

// Create the store with initial value
export const conversationStore = createConversationStore(initialState);
