<script lang="ts">
    import { onMount, onDestroy } from "svelte";
    import { chatHubStore } from "../store/chatHubStore";
    import { baseUrl } from "../baseurl";
    import { applicationStore } from "../store/applicationStore";
    import MainStructure from "./MainStructure.svelte";

    let ready = false;

    onMount(async () => {
        await chatHubStore.initialize(`${baseUrl()}/chathub`, (isUnauthorized: boolean) => isUnauthorized && applicationStore.logout());
        ready = true;
    });
    onDestroy(() => chatHubStore.dispose());
</script>


<div>
    {#if !ready}
        <div></div>
    {:else if $chatHubStore.connection}
        <MainStructure />
    {:else}
        <p>not connected</p>
    {/if}
</div>