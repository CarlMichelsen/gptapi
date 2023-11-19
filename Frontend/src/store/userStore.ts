import { writable } from 'svelte/store';
import type { UserStore } from '../types/store/userStore';
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
        const data = await getUserData();
        store.set({ user: data });
    }

    const logout = async () => {
        await deleteCookie();
        store.set({ user: null });
    }

    return {
        subscribe: store.subscribe,
        login,
        logout
    };
}

// Create the store with initial value
export const userStore = createUserStore(initialState);
