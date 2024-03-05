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

<ChatContentHolder isMessage={false}>
    <div class="relative h-36 w-full">
        <textarea
            class="resize-none h-full w-full -mb-1 focus:outline-none p-2 rounded-sm bg-zinc-700"
            on:keydown={(keyEvent) => {
                if (!keyEvent.shiftKey && keyEvent.key === "Enter") {
                    keyEvent.preventDefault();
                    sendMessage();
                }
            }}
            bind:value={content} />
        
        <button 
            class="absolute h-10 w-20 rounded-md rounded-br-none right-1 bottom-1 hover:bg-red-800 bg-green-800"
            on:click={sendMessage}>Send</button>
    </div>
</ChatContentHolder>