import { writable } from 'svelte/store';
import type { ApplicationStore, LoggedOutApplicationStore } from '../types/store/applicationStore';
import { login } from './actions/login';
import type { OmitApplicationStoreInitialParameter, StoreFunction } from './storeFunction';
import { setConversationList } from './actions/setConversationList';
import { deleteConversation } from './actions/deleteConversation';
import { logout } from './actions/logout';
import { selectConversation } from './actions/selectConversation';
import { updateConversationSummary } from './actions/updateConversationSummary';
import { receiveMessage } from './actions/receiveMessage';

// Initial state
const initialState: LoggedOutApplicationStore = {
    ready: false,
    state: "logged-out",
};

// Function to create a custom store
const createStore = () => {
    const { subscribe, update } = writable<ApplicationStore>(initialState);

    // Declare actions here
    // actionMethods should ONLY contain functions of type StoreFunction<any[]>, more specific type declaration encuraged
    const actionMethods = {
        login,
        logout,
        receiveMessage,
        selectConversation,
        setConversationList,
        updateConversationSummary,
        deleteConversation,
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