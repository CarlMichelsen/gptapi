import { writable } from 'svelte/store';
import type { UserStore } from '../types/userStore';
import { deleteCookie, getUserData } from '../clients/userDataClient';

// Initial state
const initialState: UserStore = {
    user: null,
};

// Function to create a custom store
const createUserStore = (initialValue: UserStore) => {
    // Create writable
    const store = writable<UserStore>(initialValue);

    const login = async () => {
        try {
            const data = await getUserData();
            store.set({ user: data });
        } catch (error) {
            console.error("UserStore", "login", error);
        }
    }

    const logout = async () => {
        try {
            await deleteCookie();
            store.set({ user: null });
        } catch (error) {
            console.error("UserStore", "logout", error);
        }
    }

    return {
        subscribe: store.subscribe,
        login,
        logout
    };
}

// Create the store with initial value
export const userStore = createUserStore(initialState);
