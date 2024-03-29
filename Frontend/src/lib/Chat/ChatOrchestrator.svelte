<script lang="ts">
    import { ConversationClient } from "../../clients/conversationClient";
    import { ConnectionMethods } from "../../connectionMethods";
    import { applicationStore } from "../../store/applicationStore";
    import type { AvailableModel } from "../../types/dto/availableModel/availableModel";
    import type { SendMessageRequest } from "../../types/dto/conversation/sendMessageRequest";
    import InteractionOrchestrator from "../Interaction/InteractionOrchestrator.svelte";
    import AssistantResponseParser from "./AssistantResponseParser.svelte";
    import ChatContentHolder from "./ChatContentHolder.svelte";
    import ChatHeaderBar from "./Header/ChatHeaderBar.svelte";
    import MessageContainer from "./Message/MessageContainer.svelte";
    import MessageHeader from "./Message/MessageHeader.svelte";
    import NoConversationSelected from "./NoConversationSelected.svelte";

    let streamIdentifier: string | null = null;
    let streamedMessageContent: string | null = null;

    const scrollToBottom = () => {
        const scrollableArea = document.getElementById("scrollable-chat-area-id") ?? null;
        if (scrollableArea == null) return;
        scrollableArea.scrollTop = scrollableArea.scrollHeight;
    }

    const reply = (content: string, selectedModel: AvailableModel, prevMsgId: string | null = null) => {
        if ($applicationStore.state === "logged-out") return;

        const sendMessageRequest: SendMessageRequest = {
            previousMessageId: prevMsgId,
            conversationId: $applicationStore.selectedConversation?.id ?? null,
            messageContent: content,
            promptSetting: {
                provider: selectedModel.providerIdentifier,
                model: selectedModel.modelIdentifier,
            },
        };

        ConnectionMethods.sendMessage(sendMessageRequest);
        scrollToBottom();
    };

    ConnectionMethods.receiveMessage = (receieveMessage) => {
        streamIdentifier = null;
        streamedMessageContent = null;
        applicationStore.receiveMessage(receieveMessage);
        setTimeout(scrollToBottom, 0);
    }

    ConnectionMethods.receiveMessageChunk = async (messageChunk) => {
        streamIdentifier = messageChunk.streamIdentifier;
        if (streamedMessageContent === null) {
            if ($applicationStore.state !== "logged-in") return;

            // switch to relevant conversation on first chunk received.
            if ($applicationStore.selectedConversation?.id !== messageChunk.conversationId) {
                const conversationClient = new ConversationClient();
                const res = await conversationClient.getConversation(messageChunk.conversationId);
                if (res.ok) {
                    applicationStore.selectConversation(res.data);
                }
            }

            streamedMessageContent = messageChunk.content;
        } else {
            streamedMessageContent += messageChunk.content;
        }

        scrollToBottom();
    }

    ConnectionMethods.assignSummaryToConversation = (convId: string, summary: string) => applicationStore.updateConversationSummary(convId, summary);

    ConnectionMethods.error = (err) => {
        console.error("ERROR", err);
        const desc = err.description ? `\n${err.description}` : "";
        alert(`${err.code}${desc}\n\n${err.timeStampUtc}`);
    };

    $: $applicationStore.state === "logged-in" && $applicationStore.selectedConversation && setTimeout(scrollToBottom, 0);
</script>

{#if $applicationStore.state === "logged-in"}
<div class="relative">
    <div class="absolute w-full bg-zinc-900">
        <ChatHeaderBar />
    </div>

    <div class="h-screen overflow-y-scroll no-scrollbar pb-44" id="scrollable-chat-area-id">
        {#if $applicationStore.selectedConversation !== null}
        <div>
            <ol class="space-y-8 mt-14">
            {#each $applicationStore.selectedConversation.messages as messageContainer}
                <MessageContainer messageContainer={messageContainer} />
            {/each}

            {#if streamedMessageContent && streamIdentifier}
                <li>
                    <ChatContentHolder isMessage={true} id="streaming-message">
                        <MessageHeader index={-1} messageId={streamIdentifier} role={"assistant"}/>
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

    <div class="absolute bottom-0 left-0 w-full">
        <div class="relative">
            <div class="absolute w-full bottom-32 h-0">
                <InteractionOrchestrator reply={reply} />
            </div>
        </div>
    </div>
</div>
{/if}
