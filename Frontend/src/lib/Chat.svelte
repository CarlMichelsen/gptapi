<script lang="ts">
    import { onMount, onDestroy } from "svelte";
    import { chatHubStore } from "../store/chatHubStore";
    import { localStore } from "../store/localStore";
    import { baseUrl } from "../baseurl";
    import { HubConnectionState } from "@microsoft/signalr";
    
    export let username: string;
    export let password: string;

    onMount(async () => {
        await chatHubStore.initialize(`${baseUrl()}/chathub`, username, password, (isUnauthorized: boolean) => {
            if (isUnauthorized) localStore.set({ credentials: undefined });
        });

        chatHubStore.subscribe((connectionData) => {
            if (!connectionData.connection) return;

            // Subscribe to SignalR events or call hub methods
            connectionData.connection.on("ReceiveMessage", (message) => console.log("ReceiveMessage:", message));
            connectionData.connection.on("ReceiveMessageChunk", (messageChunk) => console.log("ReceiveMessageChunk:", messageChunk));
        });
    });

    onDestroy(() => chatHubStore.dispose());
</script>

<div>
    {#if $chatHubStore.connection?.state === HubConnectionState.Connected}
        <button type="button" on:click={() => $chatHubStore.connection?.invoke("SendMessage", "Stuff")}>Send stuff</button>
    {:else}
        <p>{$chatHubStore.connection ? "connecting..." : "not connected"}</p>
    {/if}
</div>