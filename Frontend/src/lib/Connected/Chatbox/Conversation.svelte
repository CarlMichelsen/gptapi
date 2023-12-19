<script lang="ts">
    import ChatMessage from "../ChatMessage.svelte";
    import type { Message } from "../../../types/dto/message";
    import { applicationStore } from "../../../store/applicationStore";
    import NewConversation from "./NewConversation.svelte";
    import ConversationContainer from "../ConversationContainer.svelte";

    export let activeMessage: Message = {
        id: "streaming-message",
        role: "assistant",
        content: "",
        complete: false,
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

    $: setTimeout(() => onMessageChanged(activeMessage.content), 0);
</script>

{#if $applicationStore.user}
<ConversationContainer className="overflow-y-auto mx-auto" id={messageContainer}>
    <div class="container">
        {#if $applicationStore.selectedConversation}
            {#if $applicationStore.selectedConversation.summary}
                <div class="mx-2.5">
                    <h1 class="text-2xl my-2">{$applicationStore.selectedConversation.summary}</h1>
                    <hr class="mb-3">
                </div>
            {/if}
                <ol class="space-y-2 p-1">
                    {#each $applicationStore.selectedConversation.messages as message}
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
            <NewConversation />
        {/if}
    </div>
</ConversationContainer>
{/if}