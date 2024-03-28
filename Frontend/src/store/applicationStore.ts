import { writable } from 'svelte/store';
import type { ApplicationStore, LoggedOutApplicationStore } from '../types/store/applicationStore';
import type { OmitApplicationStoreInitialParameter, StoreFunction } from './storeFunction';
import { login } from './actions/login';
import { logout } from './actions/logout';
import { receiveMessage } from './actions/receiveMessage';
import { selectConversation } from './actions/selectConversation';
import { setConversationList } from './actions/setConversationList';
import { updateConversationSummary } from './actions/updateConversationSummary';
import { deleteConversation } from './actions/deleteConversation';
import { setAvailableModels } from './actions/setAvailableModels';
import { selectAvailableModel } from './actions/selectAvailableModel';


// Initial state
const initialState: LoggedOutApplicationStore = {
    ready: false,
    state: "logged-out",
    mobileSidebarVisible: false,
};

// Function to create a custom store
const createStore = () => {
    const { subscribe, update } = writable<ApplicationStore>(initialState);

    // Declare actions here
    // actionMethods should ONLY contain functions of type StoreFunction<any[]>, please don't actually use "any".
    const actionMethods = {
        login,
        logout,
        receiveMessage,
        selectConversation,
        setConversationList,
        updateConversationSummary,
        deleteConversation,
        setAvailableModels,
        selectAvailableModel,
    } as const;

    //--------------------------------------------------------------------

    type Actions = {
        [K in keyof typeof actionMethods]: (...args: OmitApplicationStoreInitialParameter<typeof actionMethods[K]>) => void;
    };

    type CreateActions<T extends Record<string, (...args: any[]) => any>> = {
        [K in keyof T]: (...args: OmitApplicationStoreInitialParameter<T[K]>) => ReturnType<T[K]>;
    };

    const createActions = <T extends Record<string, (...args: any[]) => any>>(methods: T): CreateActions<T> => {
        const actions = {} as CreateActions<T>;
        for (const key in methods) {
            actions[key] = ((...args: OmitApplicationStoreInitialParameter<typeof methods[typeof key]>) => {
                console.log(`ACTION -> ${key}`, "\n\t", ...args);
                const method = methods[key] as StoreFunction<any[]>;
                update(state => method(state, ...args));
            }) as CreateActions<T>[Extract<keyof T, string>];
        }
        return actions;
    };

    const actions: Actions = createActions(actionMethods);

    return {
        subscribe,
        ...actions,
    };
}

// Create the store with initial value
export const applicationStore = createStore();