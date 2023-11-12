<script lang="ts">
    import { onMount, onDestroy } from "svelte";
    import { signalrStore } from "../store/chatHubStore";
    import { baseUrl } from "../baseurl";
    
    export let username: string;
    export let password: string;

    onMount(async () => {
        await signalrStore.initialize(`${baseUrl()}/chathub`, username, password);

        signalrStore.subscribe((connection) => {
            if (!connection) return;

            // Subscribe to SignalR events or call hub methods
            connection.on("ReceiveMessage", (message) => console.log("Message received:", message));
        });
    });

    onDestroy(() => signalrStore.dispose());
</script>

<div>
    <p>Chat</p>
    <span>
        pwd: {password}
    </span>
</div>