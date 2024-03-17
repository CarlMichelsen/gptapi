<script lang="ts">
    import { onMount } from "svelte";
    import { applicationStore } from "../../store/applicationStore";
    import ChatContentHolder from "../Chat/ChatContentHolder.svelte";
    import { AvailableModelClient } from "../../clients/availableModelClient";
    import type { AvailableModel } from "../../types/dto/availableModel/availableModel";

    export let reply: (content: string, model: AvailableModel, prevMsgId: string | null) => void;
    let content: string = "";

    let availableModels: { [provider: string]: AvailableModel[] } | null = null;
    let selectedKey: string|null = null;
    let selectedModel: AvailableModel | null = null;

    const previousMessageId = (): string | null => {
        if ($applicationStore.state === "logged-out") return null;
        if ($applicationStore.selectedConversation == null) return null;

        const maxMsgContainer = $applicationStore.selectedConversation.messages.reduce((prev, current) => {
            return (prev && prev.index > current.index) ? prev : current
        });

        return maxMsgContainer.messageOptions[maxMsgContainer.selectedMessage]?.id ?? null;
    }

    const sendMessage = (): void => {
        if (!selectedModel) return;

        reply(content, selectedModel, previousMessageId());
        content = "";
    }

    const client = new AvailableModelClient();

    onMount(async () => {
        const res = await client.getAvailableModel();
        if (!res.ok) return;
        availableModels = res.data.availableModels;
        selectedKey = Object.keys(availableModels)[0] ?? null;
        if (selectedKey != null)
        {
            selectedModel = availableModels[selectedKey]![0] ?? null;
            selectedKey = null;
        }
    });
</script>

<ChatContentHolder isMessage={false}>
    <div class="relative h-36 w-full">
        {#if availableModels != null}
        <div class="w-full">
            <div class="absolute -top-7 w-full grid grid-cols-[1fr,1fr,2rem] gap-2">
            {#each Object.keys(availableModels) as modelKey}
                <div>
                    <button on:click={() => selectedKey = modelKey} class={`bg-zinc-700 hover:bg-zinc-500 w-full`}>{modelKey}</button>
                </div>
            {/each}

                <div>
                    <button
                        on:click={() => selectedKey = null}
                        class={`w-full h-full ${selectedKey != null ? "bg-red-500 hover:bg-red-700" : "bg-transparent text-transparent"}`}>X</button>
                </div>
            </div>

            <div class="w-full h-0 relative">
                <div class="absolute bottom-10">
                    {#if selectedKey != null && availableModels[selectedKey] != null}
                    <ul class="space-y-1">
                        {#each availableModels[selectedKey] ?? [] as model}
                        <li>
                            <button 
                                class="bg-zinc-700 hover:bg-zinc-500 w-96 text-left pl-2 py-1 rounded-md"
                                on:click={() => {selectedModel = model; selectedKey = null;}}>
                                {model.displayName}
                            </button>
                        </li>
                        {/each}
                    </ul>
                    {/if}
                </div>
            </div>

            <p class="absolute right-2 top-1 float-left text-xs text-zinc-500">{selectedModel?.displayName}</p>
        </div>
        
        {/if}

        <textarea
            class="resize-none h-full w-full -mb-1 focus:outline-none p-2 rounded-sm bg-zinc-700"
            on:focus={() => selectedKey = null}
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