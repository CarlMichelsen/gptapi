<script lang="ts">
    import type { MessageContainer } from "../../types/dto/conversation";
    import type { Message } from "../../types/dto/message";
    import MessageSelector from "./MessageSelector.svelte";
    import ChatContentHolder from "./ChatContentHolder.svelte";
    import AssistantResponseParser from "./AssistantResponseParser.svelte";

    export let messageContainer: MessageContainer;
    let msg: Message = messageContainer.messageOptions[messageContainer.selectedMessage] as Message;
    $: msg = messageContainer.messageOptions[messageContainer.selectedMessage] as Message;
</script>

<ChatContentHolder>
    <div class="grid grid-cols-[1fr_80px]">
        <p class="text-xs text-gray-300 my-auto">{messageContainer.index} - {msg.id}</p>
        <MessageSelector messageContainer={messageContainer} />
    </div>
    {#if msg.role === "assistant"}
        <AssistantResponseParser content={msg.content} />
    {:else}
        <p>{msg.content}</p>
    {/if}
</ChatContentHolder>