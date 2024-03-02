<script lang="ts">
    import { applicationStore } from "../store/applicationStore";
    import ChatStructureBox from "./Structure/ChatStructureBox.svelte";
    import ConversationStructureBox from "./Structure/ConversationStructureBox.svelte";
    import ConversationHolder from "./Chat/ConversationHolder.svelte";
    import ConversationBar from "./Conversation/ConversationBar.svelte";

    if ($applicationStore.state === "logged-in" && $applicationStore.conversations !== null) {
        console.log($applicationStore.conversations);
    }
</script>

{#if $applicationStore.state === "logged-in" && $applicationStore.conversations !== null}
    <div class="grid sm:grid-cols-[250px_1fr] sm:grid-rows-1 grid-rows-[100px_1fr] h-screen">
        <ConversationStructureBox>
            <ConversationBar />
        </ConversationStructureBox>

        <ChatStructureBox>
            <ConversationHolder />
        </ChatStructureBox>
    </div>
{/if}