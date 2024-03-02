<script lang="ts">
    import { applicationStore } from "../../store/applicationStore";
    import NoConversationSelected from "./NoConversationSelected.svelte";
    import ChatOrchestrator from "./ChatOrchestrator.svelte";


</script>

{#if $applicationStore.state === "logged-in"}
<div>
    {#if $applicationStore.selectedConversation === null}
    <NoConversationSelected />
    {:else}
    <ChatOrchestrator />
    {/if}
</div>
{/if}