<script lang="ts">
    import ChatMessage from "../ChatMessage.svelte";
    import type { ConversationType } from "../../../../types/dto/conversation";
    import type { Message } from "../../../../types/dto/message";

    export let conversation: ConversationType | null;
    export let activeMessage: Message = {
        id: "",
        role: "assistant",
        content: ""
    };
</script>

<div class="overflow-y-scroll container">
    {#if conversation}
    <div class="mx-2.5">
        <h1 class="text-2xl my-2">{conversation.summary}</h1>
        <hr class="mb-3">
    </div>
    
    <ol class="space-y-2 p-1">
        {#each conversation.messages as message}
        <li>
            <ChatMessage {message} />
        </li>
        {/each}

        {#if activeMessage.content.length > 0}
        <li>
            <ChatMessage message={activeMessage} />
        </li>
        {/if}
    </ol>
    {:else}
    <p>This is a new conversation</p>
    {/if}
</div>