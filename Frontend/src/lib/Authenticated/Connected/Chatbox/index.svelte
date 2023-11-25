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
    import { setQueryParam } from "../../../../util/queryParameters";

    export let queryConversationId: string|null;
    onMount(() => {
        setTimeout(() => {
            const detail = {
                detail: queryConversationId,
            } as CustomEvent<string|null>;
            onConversationSelected(detail);
        }, 0);
    });

    let ready: boolean = true;
    let selectedConversationId: string | null;
    let conversation: ConversationType | null;
    let activeMessage: Message;
    let chunks: MessageChunk[] = []

    const receiveMessageChunk: Exclude<typeof ConnectionMethods.receiveMessageChunk, null> = (chunk) => {
        chunks.push(chunk);
        activeMessage = { ...activeMessage, content: chunks.map(c => c.content).join("") };
    };

    const receiveMessage: Exclude<typeof ConnectionMethods.receiveMessage, null> = (message) => {
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
        ConnectionMethods.sendMessage(message, conversationId);
    }

    const onConversationSelected = async (details: CustomEvent<string|null>) => {
        setQueryParam("conv", details.detail);
        if (details.detail == null) {
            selectedConversationId = details.detail;
            conversation = details.detail;
            return;
        }

        const conv = await getConversation(details.detail);
        if (conv) {
            selectedConversationId = conv.id;
            conversation = conv;
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
    <div slot="conversation" class="grid h-screen grid-rows-[1fr,auto]">
        <Conversation
            bind:conversation={conversation}
            bind:activeMessage={activeMessage} />
        
        <InputField on:send={(message) => sendMessage(message.detail, selectedConversationId)} {ready} />
    </div>
</Structure>