<script lang="ts">
    import { onMount } from "svelte";
    import { applicationStore } from "../../store/applicationStore";
    import ChatContentHolder from "../Chat/ChatContentHolder.svelte";
    import { AvailableModelClient } from "../../clients/availableModelClient";
    import type { AvailableModel } from "../../types/dto/availableModel/availableModel";

    export let reply: (content: string, model: AvailableModel, prevMsgId: string | null) => void;
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
        if ($applicationStore.state !== "logged-in") return;
        if ($applicationStore.languageModel === null) return;

        reply(content, $applicationStore.languageModel.selectedModel, previousMessageId());
        content = "";
    }

    const initiateAvailableModels = async (): Promise<boolean> => {
        const client = new AvailableModelClient();

        const res = await client.getAvailableModel();
        if (!res.ok) return false;

        const models = res.data.availableModels;
        const provider = Object.keys(models)[0] ?? null;
        if (provider === null) return false;

        const modelList = models[provider] ?? null;
        if (modelList === null) return false;

        const model = modelList[0] ?? null;
        if (model === null) return false;

        applicationStore.setAvailableModels(models, model);
        return true;
    }

    onMount(async () => {
        const success = await initiateAvailableModels();
        if (!success) {
            console.error("Available models were not initiated");
        }
    });
</script>

<ChatContentHolder isMessage={false} id="interaction-box">
    <div class="relative h-32">
        <label for="chat-input-textarea" class="sr-only">Type your message and press Enter to send.</label>
        <textarea
            id="chat-input-textarea"
            class="resize-none h-full w-full -mb-1 focus:outline-none p-2 rounded-t-sm border shadow-xl border-zinc-800 border-b-zinc-700 bg-zinc-700"
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