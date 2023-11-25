<script lang="ts">
    import ChatMessage from "../ChatMessage.svelte";
    import type { ConversationType } from "../../../../types/dto/conversation";
    import type { Message } from "../../../../types/dto/message";

    export let conversation: ConversationType | null;
    export let activeMessage: Message = {
        id: "",
        role: "assistant",
        content: "",
        created: new Date(),
    };

    const messageContainer = "message-container-id";

    const scrollToBottom = (input?: HTMLDivElement | null) => {
        const container = (input ?? document.getElementById(messageContainer) ?? null) as HTMLDivElement | null;
        if (container) container.scrollTop = container.scrollHeight;
    }

    const onMessageChanged = (..._: any[]) => {
        const containerCandidate = document.getElementById(messageContainer);
        if (containerCandidate) {
            const container = containerCandidate as HTMLDivElement;
            const distanceFromBottom = container.scrollHeight - (container.scrollTop + container.clientHeight);
            if (distanceFromBottom < 120) {
                scrollToBottom(container);
            }
        }
    }

    $: conversation?.id, setTimeout(() => scrollToBottom(), 0);
    $: setTimeout(() => onMessageChanged(activeMessage.content, conversation?.id), 0);
</script>

<div class="overflow-y-auto" id={messageContainer}>
    <div class="container">
        {#if conversation}
            {#if conversation.summary}
                <div class="mx-2.5">
                    <h1 class="text-2xl my-2">{conversation.summary}</h1>
                    <hr class="mb-3">
                </div>
            {/if}
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
</div>