<script lang="ts">
    import { onMount, onDestroy } from "svelte";
    import { chatHubStore } from "../store/chatHubStore";
    import { baseUrl } from "../baseurl";
    import { applicationStore } from "../store/applicationStore";
    import { ConversationClient } from "../clients/conversationClient";
    import { deleteCookie } from "../clients/userDataClient";
    import Application from "./Application.svelte";

    let ready = false;

    onMount(async () => {
        await chatHubStore.initialize(`${baseUrl()}/chathub`, async (isUnauthorized: boolean) => {
            if (isUnauthorized) {
                await deleteCookie();
                applicationStore.logout();
            }
        });

        ready = true;
        const conversationClient = new ConversationClient();
        const conversationListResponse = await conversationClient.getConversationList();
        if (conversationListResponse.ok) {
            applicationStore.setConversationList(conversationListResponse.data);
        }
    });
    onDestroy(() => chatHubStore.dispose());
</script>


<div>
    {#if !ready}
        <div>
            <h3 class="text-center">...</h3>
        </div>
    {:else if $chatHubStore.connection}
        <Application />
    {:else}
        <div>
            <h3>Connection was severed refresh to attempt reconnect</h3>
        </div>
    {/if}
</div>