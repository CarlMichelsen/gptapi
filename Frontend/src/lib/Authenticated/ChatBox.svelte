<script lang="ts">
    import { onDestroy } from "svelte";
    import { ConnectionMethods } from "../../connectionMethods";
    import type { ConversationMetadata, ConversationType } from "../../types/dto/conversation";
    import Conversation from "./Conversation.svelte";
    import InputField from "./InputField.svelte";
    import Sidebar from "./Sidebar.svelte";
    import Structure from "./Structure.svelte";
    
    let selectedConversationId: string | null;

    const onNewConversation = (firstMessage: string) => {
        testData = [ { id: "guid-1", summary: "new conversation" }, ...testData ];
    }

    let testData: ConversationMetadata[] = [
        { id: "guid-1", summary: "test-conversation-1" },
    ];

    const sampleConversation: ConversationType = {
        id: null,
        summary: "Sample conversation",
        messages: []
    };

    let ready: boolean;
    let receiveMessageChunk: Exclude<typeof ConnectionMethods.receiveMessageChunk, null>;
    let receiveMessage: Exclude<typeof ConnectionMethods.receiveMessage, null>;
    let receiveFirstMessage: Exclude<typeof ConnectionMethods.receiveFirstMessage, null>;
    let sendMessage: (message: string, conversationId: string | null) => void;

    $: if (receiveMessageChunk) ConnectionMethods.receiveMessageChunk = receiveMessageChunk;
    $: if (receiveMessage) ConnectionMethods.receiveMessage = receiveMessage;
    $: if (receiveFirstMessage) ConnectionMethods.receiveFirstMessage = receiveFirstMessage;

    onDestroy(() => {
        ConnectionMethods.receiveMessageChunk = null;
        ConnectionMethods.receiveMessage = null;
    })
</script>

<div>
    <Structure>
        <Sidebar slot="sidebar" conversationOptions={testData} {onNewConversation} />
        <Conversation
            slot="conversation"
            conversation={sampleConversation}
            bind:ready={ready}
            bind:selectedConversationId={selectedConversationId}
            bind:receiveMessageChunk={receiveMessageChunk}
            bind:receiveMessage={receiveMessage}
            bind:receiveFirstMessage={receiveFirstMessage}
            bind:sendMessage={sendMessage} />
    </Structure>

    <div class="fixed inset-x-0 bottom-8">
        <Structure>
            <div slot="sidebar"></div>
            <InputField slot="conversation" onSend={(message) => sendMessage(message, selectedConversationId)} {ready} />
        </Structure>
    </div>
</div>