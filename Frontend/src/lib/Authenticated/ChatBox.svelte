<script lang="ts">
    import type { ConversationMetadata } from "../../types/dto/conversation";
    import type { Message } from "../../types/dto/message";
    import Conversation from "./Conversation.svelte";
    import InputField from "./InputField.svelte";
    import Sidebar from "./Sidebar.svelte";
    import Structure from "./Structure.svelte";

    let ready: boolean;
    let messages: Message[] = [];

    let sendMessage: (message: string) => void;

    const onNewConversation = () => {
        testData = [ { id: "guid-1", title: "new conversation" }, ...testData ];
    }

    let testData: ConversationMetadata[] = [
        { id: "guid-1", title: "test-conversation-1" },
        { id: "guid-2", title: "test-conversation-2" },
        { id: "guid-3", title: "test-conversation-3" },
    ];
</script>

<div>
    <Structure>
        <Sidebar slot="sidebar" conversationOptions={testData} {onNewConversation} />
        <Conversation slot="conversation" title="Insert title here..." bind:sendMessage={sendMessage} {messages} bind:ready={ready} />
    </Structure>

    <div class="fixed inset-x-0 bottom-8">
        <Structure>
            <div slot="sidebar"></div>
            <InputField slot="conversation" onSend={(message) => sendMessage(message)} {ready} />
        </Structure>
    </div>
</div>