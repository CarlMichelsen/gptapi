<script lang="ts">
    import { ConnectionMethods } from "../../connectionMethods";
    import { applicationStore } from "../../store/applicationStore";
    import type { SendMessageRequest } from "../../types/dto/sendMessageRequest";
    import InteractionOrchestrator from "../Interaction/InteractionOrchestrator.svelte";
    import AssistantResponseParser from "./AssistantResponseParser.svelte";
    import ChatContentHolder from "./ChatContentHolder.svelte";
    import MessageContainer from "./MessageContainer.svelte";
    import MessageHeader from "./MessageHeader.svelte";
    import NoConversationSelected from "./NoConversationSelected.svelte";

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
        scrollToBottom();
    };

    ConnectionMethods.receiveMessage = (receieveMessage) => {
        streamedMessageContent = null;
        applicationStore.receieveMessage(receieveMessage);
        setTimeout(scrollToBottom, 0);
    }

    ConnectionMethods.receiveMessageChunk = (messageChunk) => {
        if (streamedMessageContent === null) {
            applicationStore.selectConversation(messageChunk.conversationId);
            streamedMessageContent = messageChunk.content;
        } else {
            streamedMessageContent += messageChunk.content;
        }

        scrollToBottom();
    }

    $: $applicationStore.state === "logged-in" && $applicationStore.selectedConversation && setTimeout(scrollToBottom, 0);
</script>

{#if $applicationStore.state === "logged-in"}
<div class="grid grid-rows-[1fr_150px] sm:h-screen h-full">
    <div class="overflow-y-scroll no-scrollbar pb-10" id="scrollable-chat-area-id">
        {#if $applicationStore.selectedConversation !== null}
        <div>
            {#if $applicationStore.selectedConversation.summary !== null}
            <h1>{$applicationStore.selectedConversation.summary}</h1>
            {/if}
        </div>

        <div>
            <ol class="space-y-10">
            {#each $applicationStore.selectedConversation.messages as messageContainer}
                <li>
                    <MessageContainer messageContainer={messageContainer} />
                </li>
            {/each}

            {#if streamedMessageContent}
                <li>
                    <ChatContentHolder>
                        <MessageHeader index={-1} messageId="ongoing..." role={"assistant"}/>
                        <AssistantResponseParser content={streamedMessageContent} />
                    </ChatContentHolder>
                </li>
            {/if}
            </ol>
        </div>
        {:else}
        <NoConversationSelected />
        {/if}
    </div>

    <InteractionOrchestrator reply={reply} />
</div>
{/if}
