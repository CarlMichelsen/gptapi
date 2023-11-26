<script lang="ts">
    import { onDestroy, onMount } from "svelte";
    import { ConnectionMethods } from "../../connectionMethods";
    import Chatbox from "./Chatbox/index.svelte";
    import { applicationStore } from "../../store/applicationStore";

    onMount(() => {
        ConnectionMethods.assignSummaryToConversation = (conversationId: string, summary: string) => {
            applicationStore.updateConversationSummary(conversationId, summary);
        }
    });

    onDestroy(() => {
        ConnectionMethods.assignSummaryToConversation = null;
    });
</script>

<Chatbox />