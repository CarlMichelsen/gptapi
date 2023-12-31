import { writable } from 'svelte/store';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { ConnectionMethods } from '../connectionMethods';
import type { ChatHubStore } from '../types/store/chatHubStore';

const initial: ChatHubStore = {
    connection: null,
};

// Create a writable store
const createStore = () => {
    const { subscribe, set } = writable<ChatHubStore>(initial);

    let connection: HubConnection | null = null;

    // Initialize connection
    const initialize = async (hubUrl: string, failure?: (isUnauthorized: boolean) => void) => {
        // Dispose existing connection if any
        await dispose();

        connection = new HubConnectionBuilder()
            .withUrl(hubUrl)
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .build();

        connection.onclose(() => {
            console.log('SignalR connection closed');
            set({ connection: null });
        });

        try {
            await connection.start();
            ConnectionMethods.connection = connection;
            ConnectionMethods.disconnect = () => {
                connection?.stop();
            }
            console.log('SignalR connection started');
            set({ connection });
        } catch (err: unknown) {
            const error = err as Error;
            console.error('Error while establishing SignalR connection:', error.message);
            if (failure) failure(error.message.indexOf('401') !== -1);
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
