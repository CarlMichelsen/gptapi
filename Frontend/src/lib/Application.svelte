<script lang="ts">
    import ChatOrchestrator from "./Chat/ChatOrchestrator.svelte";
    import ConversationBar from "./Conversation/ConversationBar.svelte";
</script>

<div class="hidden md:block">
    <div class="grid grid-cols-[250px_1fr]">
        <ConversationBar />
        <ChatOrchestrator />
    </div>
</div>

<div class="block md:hidden">
    <div>
        <ChatOrchestrator />
    </div>
</div>
