<script lang="ts">
    import { ConnectionMethods } from "../../connectionMethods";
    import type { ConversationType } from "../../types/dto/conversation";
    import type { Message } from "../../types/dto/message";
    import type { MessageChunk } from "../../types/dto/messageChunk";
    import ChatMessage from "./ChatMessage.svelte";

    export let selectedConversationId: string|null = null;
    export let ready: boolean = true;
    export let conversation: ConversationType;

    export const receiveMessageChunk: Exclude<typeof ConnectionMethods.receiveMessageChunk, null> = (chunk) => {
        console.log("messageChunk", chunk);
        chunks.push(chunk);
        activeMessage = { ...activeMessage, content: chunks.map(c => c.content).join("") };
    };

    export const receiveMessage: Exclude<typeof ConnectionMethods.receiveMessage, null> = (message) => {
        console.log("message", message);
        activeMessage = { ...activeMessage, content: "" };
        conversation.messages = [...conversation.messages, message];
        ready = true;
    };

    export const receiveFirstMessage: Exclude<typeof ConnectionMethods.receiveFirstMessage, null> = (firstMessage) => {
        selectedConversationId = firstMessage.conversationId;
        conversation.messages = [...conversation.messages, firstMessage.message];
    }

    export const sendMessage = (message: string, conversationId: string | null) => {
        chunks = [];
        activeMessage = { ...activeMessage, content: "" };
        ready = false;
        console.log(message, conversationId);
        ConnectionMethods.sendMessage(message, conversationId);
    }

    let chunks: MessageChunk[] = []

    let activeMessage: Message = {
        id: "none",
        role: "assistant",
        content: "",
    };
</script>

<div class="overflow-y-scroll container">
    <h1 class="text-2xl my-2">{conversation.summary}</h1>
    <hr class="mb-3">

    <ol class="space-y-2 p-1">
        {#each conversation.messages as message}
            <li>
                <ChatMessage {message} />
            </li>
        {/each}

        {#if activeMessage && activeMessage.content.length > 0}
            <li>
                <ChatMessage message={activeMessage} />
            </li>
        {/if}
    </ol>
</div>