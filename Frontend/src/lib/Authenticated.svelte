<script lang="ts">
    import { onMount, onDestroy } from "svelte";
    import { chatHubStore } from "../store/chatHubStore";
    import { baseUrl } from "../baseurl";
    import { applicationStore } from "../store/applicationStore";
    import MainStructure from "./MainStructure.svelte";
    import { ConversationClient } from "../clients/conversationClient";
    import { deleteCookie } from "../clients/userDataClient";
    import ConversationBar from "./Conversation/ConversationBar.svelte";
    import ChatOrchestrator from "./Chat/ChatOrchestrator.svelte";

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
        <div></div>
    {:else if $chatHubStore.connection}
        <MainStructure>
            <ConversationBar slot="sidebar" />
            <ChatOrchestrator slot="chat" />
        </MainStructure>
    {:else}
        <p>not connected</p>
    {/if}
</div>