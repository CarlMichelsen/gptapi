<script lang="ts">
    import { onDestroy, onMount } from "svelte";
    import { ConnectionMethods } from "../../../connectionMethods";
    import type { MessageChunk } from "../../../types/dto/messageChunk";
    import Sidebar from "../Sidebar.svelte";
    import Structure from "../Structure.svelte";
    import Conversation from "./Conversation.svelte";
    import InputField from "../InputField.svelte";
    import { applicationStore } from "../../../store/applicationStore";
    import type { Message } from "../../../types/dto/message";

    let ready: boolean = true;
    let activeMessage: Message;
    let chunks: MessageChunk[] = []

    const sendMessage = (message: string) => {
        if (!$applicationStore.user) return;

        chunks = [];
        activeMessage = { ...activeMessage, content: "" };
        ready = false;
        ConnectionMethods.sendMessage(message, $applicationStore.selectedConversation?.id ?? null);
    }

    onMount(() => {
        ConnectionMethods.receiveMessageChunk = (chunk) => {
            chunks.push(chunk);
            activeMessage = { ...activeMessage, content: chunks.map(c => c.content).join("") };
        };

        ConnectionMethods.receiveMessage = (message) => {
            chunks = [];
            activeMessage = { ...activeMessage, content: "" };
            ready = true;
            applicationStore.receieveMessage(message);
        };

        ConnectionMethods.receiveFirstMessage = async (firstMessage) =>
            applicationStore.addNewConversation(firstMessage.conversationId);
    });

    onDestroy(() => {
        ConnectionMethods.receiveMessageChunk = null;
        ConnectionMethods.receiveMessage = null;
        ConnectionMethods.receiveFirstMessage = null;
    });
</script>

<Structure>
    <Sidebar slot="sidebar" on:conversationSelected={(details) => applicationStore.selectConversation(details.detail)} />
    <div slot="conversation" class="grid h-screen grid-rows-[1fr,auto]">
        <Conversation bind:activeMessage={activeMessage} />
        <InputField on:send={(message) => sendMessage(message.detail)} {ready} />
    </div>
</Structure>