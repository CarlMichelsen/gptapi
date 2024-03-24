<script lang="ts">
    import type { MessageContainer } from "../../types/dto/conversation/conversation";
    import type { Message } from "../../types/dto/conversation/message";

    export let messageContainer: MessageContainer;

    const getMessageList = (messageContainer: MessageContainer): Message[] => {
        return Object
            .values(messageContainer.messageOptions)
            .sort((m1, m2) => (new Date(m1.createdUtc)).getTime()-(new Date(m2.createdUtc)).getTime());
    }
    let messageList = getMessageList(messageContainer);
    $: messageList = getMessageList(messageContainer);


    let selectedMessageId: number = messageList.findIndex((msg) => msg.id === messageContainer.selectedMessage);
    $: selectedMessageId = messageList.findIndex((msg) => msg.id === messageContainer.selectedMessage);

    let maxMessageId: number = messageList.length;
</script>

<div class="grid grid-cols-3 h-8">
    <button class="font-bold text-md hover:text-lg h-full">{"<"}</button>
    <p class="my-auto">{selectedMessageId+1}/{maxMessageId}</p>
    <button class="font-bold text-md hover:text-lg h-full">{">"}</button>
</div>