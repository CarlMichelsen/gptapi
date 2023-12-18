<script lang="ts">
    import { onMount, onDestroy } from "svelte";
    import { chatHubStore } from "../store/chatHubStore";
    import { baseUrl } from "../baseurl";
    import Connected from "./Connected/index.svelte";
    import { applicationStore } from "../store/applicationStore";
    import Spinner from "./Spinner.svelte";

    let ready = false;

    onMount(async () => {
        await chatHubStore.initialize(`${baseUrl()}/chathub`, (isUnauthorized: boolean) => isUnauthorized && applicationStore.logout());
        ready = true;
    });
    onDestroy(() => chatHubStore.dispose());
</script>


<div>
    {#if !ready}
        <div>
            <Spinner className="mt-32 mx-auto" />
        </div>
    {:else if $chatHubStore.connection}
        <Connected />
    {:else}
        <p>not connected</p>
    {/if}
</div>