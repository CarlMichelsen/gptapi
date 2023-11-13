<script lang="ts">
    import { onMount, onDestroy } from "svelte";
    import { chatHubStore } from "../store/chatHubStore";
    import { localStore } from "../store/localStore";
    import { baseUrl } from "../baseurl";
    import ChatBox from "./ChatBox.svelte";
    
    export let username: string;
    export let password: string;

    onMount(async () => {
        await chatHubStore.initialize(`${baseUrl()}/chathub`, username, password, (isUnauthorized: boolean) => {
            if (isUnauthorized) localStore.set({ credentials: undefined });
        });
    });

    onDestroy(() => chatHubStore.dispose());
</script>

<div>
    {#if $chatHubStore.connection}
        <ChatBox />
    {:else}
        <p>not connected</p>
    {/if}
</div>