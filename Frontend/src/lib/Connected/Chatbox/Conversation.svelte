<script lang="ts">
    import ChatMessage from "../ChatMessage.svelte";
    import type { Message } from "../../../types/dto/message";
    import { applicationStore } from "../../../store/applicationStore";
    import NewConversation from "./NewConversation.svelte";
    import ConversationContainer from "../ConversationContainer.svelte";

    export let activeMessage: Message = {
        id: "streaming-message",
        previousMessageId: null,
        role: "assistant",
        content: "",
        complete: false,
        created: new Date(),
    };

    const messageContainerId = "message-container-id";

    const scrollToBottom = (input?: HTMLDivElement | null) => {
        const container = (input ?? document.getElementById(messageContainerId) ?? null) as HTMLDivElement | null;
        if (container) container.scrollTop = container.scrollHeight;
    }

    const onMessageChanged = (..._: any[]) => {
        const containerCandidate = document.getElementById(messageContainerId);
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

{#if $applicationStore.state === "logged-in"}
<ConversationContainer className="overflow-y-auto mx-auto" id={messageContainerId}>
    <div class="container">
        {#if $applicationStore.selectedConversation}
                <ol class="space-y-2 p-1">
                    {#each $applicationStore.selectedConversation.messages as messageContainer}
                    <li>
                        <ChatMessage message={messageContainer.messageOptions?.get(messageContainer.selectedMessage) ?? []} />
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