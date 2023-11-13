import type { HubConnection } from "@microsoft/signalr";

export type ChatHubStore = {
    connection: HubConnection | null;
};