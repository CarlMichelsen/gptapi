import { writable } from 'svelte/store';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

// Create a writable store
const createStore = () => {
    const { subscribe, set } = writable<HubConnection | null>(null);

    let connection: HubConnection | null = null;

    // Initialize connection
    const initialize = async (hubUrl: string, username:string, password: string) => {
        // Dispose existing connection if any
        await dispose();

        // Encode username and password for Basic Auth
        const encodedCredentials = btoa(`${username}:${password}`);

        connection = new HubConnectionBuilder()
            .withUrl(`${hubUrl}?access_token=${encodedCredentials}`)
            .configureLogging(LogLevel.Information)
            .build();

        connection.onclose(() => {
            console.log('SignalR connection closed');
            set(null);
        });

        try {
            await connection.start();
            console.log('SignalR connection started');
            set(connection);
        } catch (err) {
            console.error('Error while establishing SignalR connection:', err);
        }
    };

    // Dispose connection
    const dispose = async () => {
        if (connection) {
            await connection.stop();
            connection = null;
            set(null);
        }
    };

    return {
        subscribe,
        initialize,
        dispose,
    };
};

export const signalrStore = createStore();
