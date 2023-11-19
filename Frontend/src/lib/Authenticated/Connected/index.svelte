<script lang="ts">
    import { onDestroy } from "svelte";
    import { ConnectionMethods } from "../../../connectionMethods";
    import Chatbox from "./Chatbox/index.svelte";
    import { conversationStore } from "../../../store/conversationStore";
    import { getConversationOptions } from "../../../clients/conversationClient";

    const fetchConversationList = async () => {
        const options = await getConversationOptions();
        if (options) conversationStore.assignOptions(options);
    }

    fetchConversationList();

    let receiveMessageChunk: Exclude<typeof ConnectionMethods.receiveMessageChunk, null>;
    let receiveMessage: Exclude<typeof ConnectionMethods.receiveMessage, null>;
    let receiveFirstMessage: Exclude<typeof ConnectionMethods.receiveFirstMessage, null>;

    $: if (receiveMessageChunk) ConnectionMethods.receiveMessageChunk = receiveMessageChunk;
    $: if (receiveMessage) ConnectionMethods.receiveMessage = receiveMessage;
    $: if (receiveFirstMessage) ConnectionMethods.receiveFirstMessage = receiveFirstMessage;

    onDestroy(() => {
        ConnectionMethods.receiveMessageChunk = null;
        ConnectionMethods.receiveMessage = null;
    })
</script>

<Chatbox />