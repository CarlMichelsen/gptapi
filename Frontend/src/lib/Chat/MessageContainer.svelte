<script lang="ts">
    import type { MessageContainer } from "../../types/dto/conversation";
    import type { Message } from "../../types/dto/message";
    import ChatContentHolder from "./ChatContentHolder.svelte";
    import AssistantResponseParser from "./AssistantResponseParser.svelte";
    import MessageHeader from "./MessageHeader.svelte";

    export let messageContainer: MessageContainer;
    let msg: Message = messageContainer.messageOptions[messageContainer.selectedMessage] as Message;
    $: msg = messageContainer.messageOptions[messageContainer.selectedMessage] as Message;
</script>

{#if msg.visible}
<li>
    <ChatContentHolder isMessage={true} id={msg.id}>
        <MessageHeader index={messageContainer.index} messageId={msg.id} role={msg.role}/>

        <div>
            {#if msg.role === "assistant"}
                <AssistantResponseParser content={msg.content} />
            {:else}
                <pre class="font-sans w-full overflow-auto whitespace-break-spaces">{msg.content}</pre>
            {/if}
        </div>
    </ChatContentHolder>
</li>
{/if}