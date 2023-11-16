<script lang="ts">
    import { onMount, onDestroy } from "svelte";
    import { chatHubStore } from "../store/chatHubStore";
    import { localStore } from "../store/localStore";
    import { baseUrl } from "../baseurl";
    import ChatBox from "./ChatBox.svelte";

    onMount(async () => {
        await chatHubStore.initialize(`${baseUrl()}/chathub`, (isUnauthorized: boolean) => {
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