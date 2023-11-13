import { writable } from 'svelte/store';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import type { ChatHubStore } from '../types/chatHubStore';

const initial: ChatHubStore = {
    connection: null,
};

// Create a writable store
const createStore = () => {
    const { subscribe, set } = writable<ChatHubStore>(initial);

    let connection: HubConnection | null = null;

    // Initialize connection
    const initialize = async (hubUrl: string, username:string, password: string, failure?: (isUnauthorized: boolean) => void) => {
        // Dispose existing connection if any
        await dispose();

        // Encode username and password for Basic Auth
        const encodedCredentials = btoa(`${username}:${password}`);

        connection = new HubConnectionBuilder()
            .withUrl(`${hubUrl}?access_token=${encodedCredentials}`)
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .build();

        connection.onclose(() => {
            console.log('SignalR connection closed');
            set({ connection: null });
        });

        try {
            await connection.start();
            connection.on("Disconnect", () => connection?.stop());
            console.log('SignalR connection started');
            set({ connection });
        } catch (err: unknown) {
            const error = err as Error;

            console.error('Error while establishing SignalR connection:', error.message);

            failure && failure(error.message.indexOf('401') !== -1);
        }
    };

    // Dispose connection
    const dispose = async () => {
        if (connection) {
            await connection.stop();
            connection = null;
            set({ connection: null });
        }
    };

    return {
        subscribe,
        initialize,
        dispose,
    };
};

export const chatHubStore = createStore();
