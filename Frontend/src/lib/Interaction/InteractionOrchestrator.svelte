<script lang="ts">
    import { applicationStore } from "../../store/applicationStore";
    import ChatContentHolder from "../Chat/ChatContentHolder.svelte";

    export let reply: (content: string, prevMsgId: string | null) => void;
    let content: string = "";

    const previousMessageId = (): string | null => {
        if ($applicationStore.state === "logged-out") return null;
        if ($applicationStore.selectedConversation == null) return null;

        const maxMsgContainer = $applicationStore.selectedConversation.messages.reduce((prev, current) => {
            return (prev && prev.index > current.index) ? prev : current
        });

        return maxMsgContainer.messageOptions[maxMsgContainer.selectedMessage]?.id ?? null;
    }

    const sendMessage = (): void => {
        reply(content, previousMessageId());
        content = "";
    }
</script>

<ChatContentHolder>
    <div class="relative h-24 sm:h-36 w-full">
        <textarea
            class="resize-none h-full w-full -mb-1 focus:outline-none p-2 rounded-sm"
            bind:value={content} />
        
        <button 
            class="absolute h-14 w-24 rounded-full rounded-br-none right-1 bottom-1 bg-red-400 hover:bg-green-400"
            on:click={sendMessage}>Send</button>
    </div>
</ChatContentHolder>