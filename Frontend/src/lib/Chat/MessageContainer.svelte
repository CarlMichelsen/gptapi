<script lang="ts">
    import type { MessageContainer } from "../../types/dto/conversation";
    import type { Message } from "../../types/dto/message";
    import { displayDate } from "../../util/dateParse";
    import MessageSelector from "./MessageSelector.svelte";
    import ChatContentHolder from "./ChatContentHolder.svelte";
    import AssistantResponseParser from "./AssistantResponseParser.svelte";
    import MessageHeader from "./MessageHeader.svelte";

    export let messageContainer: MessageContainer;
    let msg: Message = messageContainer.messageOptions[messageContainer.selectedMessage] as Message;
    $: msg = messageContainer.messageOptions[messageContainer.selectedMessage] as Message;
</script>

<ChatContentHolder>
    <div class="grid grid-cols-[1fr_80px]">
        <MessageHeader index={messageContainer.index} messageId={msg.id} dateText={displayDate(msg.completedUtc)}/>
        <MessageSelector messageContainer={messageContainer} />
    </div>
    <div class="p-1">
        {#if msg.role === "assistant"}
            <AssistantResponseParser content={msg.content} />
        {:else}
            <pre class="font-sans w-full overflow-auto">{msg.content}</pre>
        {/if}
    </div>
</ChatContentHolder>