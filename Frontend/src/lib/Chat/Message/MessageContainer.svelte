<script lang="ts">
    import type { MessageContainer } from "../../../types/dto/conversation/conversation";
    import type { Message } from "../../../types/dto/conversation/message";
    import ChatContentHolder from "../ChatContentHolder.svelte";
    import MessageHeader from "./MessageHeader.svelte";
    import MessageTextContent from "./MessageTextContent.svelte";

    export let messageContainer: MessageContainer;
    let msg: Message = messageContainer.messageOptions[messageContainer.selectedMessage] as Message;
    $: msg = messageContainer.messageOptions[messageContainer.selectedMessage] as Message;
</script>

{#if msg.visible}
<li>
    <ChatContentHolder isMessage={true} id={msg.id}>
        <MessageHeader index={messageContainer.index} messageId={msg.id} role={msg.role}/>
        <div class="ml-8">
            <MessageTextContent message={msg}/>
        </div>
    </ChatContentHolder>
</li>
{/if}