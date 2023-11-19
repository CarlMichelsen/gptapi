<script lang="ts">
    import { onMount, onDestroy } from "svelte";
    import { chatHubStore } from "../../store/chatHubStore";
    import { baseUrl } from "../../baseurl";
    import Connected from "./Connected/index.svelte";
    import type { SteamPlayer } from "../../types/dto/steamPlayer";

    export let userData: SteamPlayer;

    onMount(async () => {
        await chatHubStore.initialize(`${baseUrl()}/chathub`, (isUnauthorized: boolean) => {
            // if (isUnauthorized) localStore.set({ credentials: undefined });
            // user is not logged in. Handle this.
        });

        console.log("userData", userData);
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