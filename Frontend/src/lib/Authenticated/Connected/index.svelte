<script lang="ts">
    import { onDestroy, onMount } from "svelte";
    import { ConnectionMethods } from "../../../connectionMethods";
    import Chatbox from "./Chatbox/index.svelte";
    import { conversationStore } from "../../../store/conversationStore";
    import { getConversationOptions } from "../../../clients/conversationClient";

    const fetchConversationList = async () => {
        const options = await getConversationOptions();
        if (options) conversationStore.assignOptions(options);
    }

    fetchConversationList();

    onMount(() => {
        ConnectionMethods.assignSummaryToConversation = (conversationId: string, summary: string) => {
            conversationStore.updateSummary(conversationId, summary);
        }
    });

    onDestroy(() => {
        ConnectionMethods.assignSummaryToConversation = null;
    });
</script>

<Chatbox />