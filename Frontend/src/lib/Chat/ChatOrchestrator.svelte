<script lang="ts">
    import { ConnectionMethods } from "../../connectionMethods";
    import { applicationStore } from "../../store/applicationStore";
    import type { SendMessageRequest } from "../../types/dto/sendMessageRequest";
    import InteractionOrchestrator from "../Interaction/InteractionOrchestrator.svelte";
    import ChatContentHolder from "./ChatContentHolder.svelte";
    import MessageContainer from "./MessageContainer.svelte";

    let streamedMessageContent: string | null = null;

    const scrollToBottom = () => {
        const scrollableArea = document.getElementById("scrollable-chat-area-id") ?? null;
        if (scrollableArea == null) return;
        scrollableArea.scrollTop = scrollableArea.scrollHeight;
    }

    const reply = (content: string, prevMsgId: string | null = null) => {
        if ($applicationStore.state === "logged-out") return;

        const sendMessageRequest: SendMessageRequest = {
            previousMessageId: prevMsgId,
            conversationId: $applicationStore.selectedConversation?.id ?? null,
            messageContent: content,
        };

        ConnectionMethods.sendMessage(sendMessageRequest);
    };

    ConnectionMethods.receiveMessage = (receieveMessage) => {
        streamedMessageContent = null;
        applicationStore.receieveMessage(receieveMessage);
        scrollToBottom();
    }

    ConnectionMethods.receiveMessageChunk = (messageChunk) => {
        if (streamedMessageContent === null) {
            streamedMessageContent = messageChunk.content;
        } else {
            streamedMessageContent += messageChunk.content;
        }

        scrollToBottom();
    }

    setTimeout(scrollToBottom, 0);
</script>

{#if $applicationStore.state === "logged-in" && $applicationStore.selectedConversation !== null}
<div class="grid grid-rows-[1fr_150px] h-screen">
    <div class="overflow-y-scroll pb-10" id="scrollable-chat-area-id">
        <div>
            {#if $applicationStore.selectedConversation.summary !== null}
            <h1>{$applicationStore.selectedConversation.summary}</h1>
            {/if}
        </div>

        <div>
            <ol class="space-y-1">
            {#each $applicationStore.selectedConversation.messages as messageContainer}
                <li>
                    <MessageContainer messageContainer={messageContainer} />
                </li>
            {/each}

            {#if streamedMessageContent}
                <li>
                    <ChatContentHolder>
                        <p>{streamedMessageContent}</p>
                    </ChatContentHolder>
                </li>
            {/if}
            </ol>
        </div>
    </div>

    <InteractionOrchestrator reply={reply} />
</div>
{/if}
