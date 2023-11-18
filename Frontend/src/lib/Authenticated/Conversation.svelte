<script lang="ts">
    import { onDestroy, onMount } from "svelte";
    import { ConnectionMethods } from "../../connectionMethods";
    import type { Message } from "../../types/dto/message";
    import type { MessageChunk } from "../../types/dto/messageChunk";
    import ChatMessage from "./ChatMessage.svelte";

    export let ready = true;
    export let title: string;
    export let messages: Message[] = [];

    let chunks: MessageChunk[] = []
    let activeMessage: Message = {
        id: "none",
        role: "assistant",
        content: "",
    };

    export const sendMessage = (message: string) => {
        chunks = [];
        activeMessage = { ...activeMessage, content: "" };
        const userMessage: Message = {
            id: "none",
            role: "user",
            content: message,
        };
        messages = [...messages, userMessage];
        ready = false;
        ConnectionMethods.sendMessage(message);
    }

    onMount(() => {
        ConnectionMethods.receiveMessageChunk = async (messageChunk) => {
            console.log("messageChunk", messageChunk);
            chunks.push(messageChunk);
            activeMessage = { ...activeMessage, content: chunks.map(c => c.content).join("") };
        }

        ConnectionMethods.receiveMessage = async (message) => {
            console.log("message", message);
            activeMessage = { ...activeMessage, content: "" };
            messages = [...messages, message];
            ready = true;
        }
    });

    onDestroy(() => {
        ConnectionMethods.receiveMessageChunk = null;
        ConnectionMethods.receiveMessage = null;
    })
</script>

<div class="overflow-y-scroll container">
    <h1 class="text-2xl my-2">{title}</h1>
    <hr class="mb-3">

    <ol class="space-y-2 p-1">
        {#each messages as message}
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
</div>