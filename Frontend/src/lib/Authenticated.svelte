<script lang="ts">
    import { onMount, onDestroy } from "svelte";
    import { chatHubStore } from "../store/chatHubStore";
    import { baseUrl } from "../baseurl";
    import Connected from "./Connected/index.svelte";
    import { applicationStore } from "../store/applicationStore";

    onMount(async () => {
        await chatHubStore.initialize(`${baseUrl()}/chathub`, (isUnauthorized: boolean) => isUnauthorized && applicationStore.logout());
    });
    onDestroy(() => chatHubStore.dispose());
</script>

<div>
    {#if $chatHubStore.connection}
        <Connected />
    {:else}
        <p>not connected</p>
    {/if}
</div>