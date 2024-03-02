<script lang="ts">
    import { applicationStore } from "../store/applicationStore";
    import ChatStructureBox from "./Structure/ChatStructureBox.svelte";
    import ConversationStructureBox from "./Structure/ConversationStructureBox.svelte";
    import ConversationOptionList from "./Conversation/ConversationOptionList.svelte";
    import ConversationHolder from "./Chat/ConversationHolder.svelte";

    if ($applicationStore.state === "logged-in" && $applicationStore.conversations !== null) {
        console.log($applicationStore.conversations);
    }
</script>

{#if $applicationStore.state === "logged-in" && $applicationStore.conversations !== null}
    <div class="grid sm:grid-cols-[250px_1fr] sm:grid-rows-1 grid-rows-[50px_1fr] h-screen">
        <ConversationStructureBox>
            <ConversationOptionList conversationOptionList={$applicationStore.conversations} />
        </ConversationStructureBox>

        <ChatStructureBox>
            <ConversationHolder />
        </ChatStructureBox>
    </div>
{/if}