import { writable } from 'svelte/store';
import type { LocalStore } from '../types/store/localStore';

// Initial state
const initialState: LocalStore = {
    credentials: undefined,
};

// Function to create a custom store
const createLocalStorageStore = <T>(key: string, initialValue: T) => {
    // Get the value from localStorage and parse it
    const storedValue = JSON.parse(localStorage.getItem(key) ?? 'null');

    // Use stored value if it exists, otherwise use the initial value
    const store = writable<T>(storedValue ?? initialValue);

    // Subscribe to changes in the store and update localStorage
    store.subscribe((value) => {
        localStorage.setItem(key, JSON.stringify(value));
    });

    return store;
}

// Create the store with initial value
export const localStore = createLocalStorageStore<LocalStore>('LocalStoreKey', initialState);
