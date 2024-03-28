import { writable } from 'svelte/store';
import type { LocalStore } from '../types/store/localStore';

// Initial state
const initialState: LocalStore = {
    preferences: null,
};

// Function to create a custom store
const createStore = (initialValue: LocalStore) => {
    const { subscribe, update } = writable<LocalStore>(initialState);
    const key = "LocalStoreKey";

    // Get the value from localStorage and parse it
    const storedValue = JSON.parse(localStorage.getItem(key) ?? 'null') ?? initialValue;
    update(() => storedValue);

    // Subscribe to changes in the store and update localStorage
    subscribe((value) => {
        localStorage.setItem(key, JSON.stringify(value));
    });

    return {
        subscribe,
        update,
    };
}

// Create the store with initial value
export const localStore = createStore(initialState);
