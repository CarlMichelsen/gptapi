<script lang="ts">
    import { onDestroy, onMount } from "svelte";
    import { ConnectionMethods } from "../../../connectionMethods";
    import type { MessageChunk } from "../../../types/dto/messageChunk";
    import Sidebar from "../Sidebar.svelte";
    import MobileSidebar from "../MobileSidebar.svelte";
    import Structure from "../Structure.svelte";
    import Conversation from "./Conversation.svelte";
    import InputField from "../InputField.svelte";
    import { applicationStore } from "../../../store/applicationStore";
    import type { Message } from "../../../types/dto/message";
    import type { SendMessageRequest } from "../../../types/dto/sendMessageRequest";

    let ready: boolean = true;
    let activeMessage: Message;
    let chunks: MessageChunk[] = []

    const sendMessage = (message: string, branchFromMessageId: string|null = null) => {
        if ($applicationStore.state !== "logged-in") return;
        chunks = [];
        activeMessage = { ...activeMessage, content: "" };
        ready = false;

        const sendMessageRequest: SendMessageRequest = {
            temporaryUserMessageId: "Hello",
            messageContent: message,
            conversationId: $applicationStore.selectedConversation?.id ?? null,
            previousMessageId: branchFromMessageId,
        };

        ConnectionMethods.sendMessage(sendMessageRequest);
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

{#if $applicationStore.state === "logged-in"}
<Structure>
    <div slot="sidebar" class="h-full">
        <div class="hidden md:block">
            <Sidebar />
        </div>
        <div class="block md:hidden h-full">
            <MobileSidebar />
        </div>
    </div>
    <div slot="conversation" class="grid height-screen-minus-50 md:h-screen grid-rows-[1fr,auto]">
        <Conversation bind:activeMessage={activeMessage} />
        <InputField on:send={(message) => sendMessage(message.detail)} {ready} />
    </div>
</Structure>
{/if}