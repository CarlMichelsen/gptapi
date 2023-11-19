<script lang="ts">
    import { onDestroy, onMount } from "svelte";
    import { getConversation } from "../../../../clients/conversationClient";
    import { ConnectionMethods } from "../../../../connectionMethods";
    import { conversationStore } from "../../../../store/conversationStore";
    import type { ConversationMetadata, ConversationType } from "../../../../types/dto/conversation";
    import type { Message } from "../../../../types/dto/message";
    import type { MessageChunk } from "../../../../types/dto/messageChunk";
    import Sidebar from "../Sidebar.svelte";
    import Structure from "../Structure.svelte";
    import Conversation from "./Conversation.svelte";
    import InputField from "../InputField.svelte";

    let ready: boolean = true;
    let selectedConversationId: string | null;
    let conversation: ConversationType | null;
    let activeMessage: Message;
    let chunks: MessageChunk[] = []

    const receiveMessageChunk: Exclude<typeof ConnectionMethods.receiveMessageChunk, null> = (chunk) => {
        console.log("messageChunk", chunk);
        chunks.push(chunk);
        activeMessage = { ...activeMessage, content: chunks.map(c => c.content).join("") };
    };

    const receiveMessage: Exclude<typeof ConnectionMethods.receiveMessage, null> = (message) => {
        console.log("message", message);
        if (!conversation) return;
        activeMessage = { ...activeMessage, content: "" };
        conversation.messages = [...conversation.messages, message];
        ready = true;
    };

    const receiveFirstMessage: Exclude<typeof ConnectionMethods.receiveFirstMessage, null> = async (firstMessage) => {
        const conv = await getConversation(firstMessage.conversationId);
        if (!conv) return;
        conv.messages = conv.messages.filter(m => m.content.trim() !== "");
        conversationStore.addOption({
            id: conv.id,
            summary: "null",
            created: new Date(),
        } as ConversationMetadata);
        selectedConversationId = conv.id;
        conversation = conv;
    }

    export const sendMessage = (message: string, conversationId: string | null) => {
        chunks = [];
        activeMessage = { ...activeMessage, content: "" };
        ready = false;
        console.log(message, conversationId);
        ConnectionMethods.sendMessage(message, conversationId);
    }

    const onConversationSelected = async (details: CustomEvent<string>) => {
        const conv = await getConversation(details.detail);
        if (conv) {
            selectedConversationId = conv.id;
            conversation = conv;
            console.log("conversationSelected", conv);
        }
    } 

    onMount(() => {
        ConnectionMethods.receiveMessageChunk = receiveMessageChunk;
        ConnectionMethods.receiveMessage = receiveMessage;
        ConnectionMethods.receiveFirstMessage = receiveFirstMessage;
    });

    onDestroy(() => {
        ConnectionMethods.receiveMessageChunk = null;
        ConnectionMethods.receiveMessage = null;
        ConnectionMethods.receiveFirstMessage = null;
    });
</script>

<Structure>
    <Sidebar
        slot="sidebar"
        conversationOptions={$conversationStore.conversationOptions}
        bind:selectedConversationId={selectedConversationId}
        on:conversationSelected={onConversationSelected} />
    <Conversation
        slot="conversation"
        bind:conversation={conversation}
        bind:activeMessage={activeMessage} />
</Structure>

<div class="fixed inset-x-0 bottom-8">
    <Structure>
        <div slot="sidebar"></div>
        <InputField slot="conversation" on:send={(message) => sendMessage(message.detail, selectedConversationId)} {ready} />
    </Structure>
</div>