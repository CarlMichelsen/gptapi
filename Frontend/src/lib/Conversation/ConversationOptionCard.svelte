<script lang="ts">
    import type { MouseEventHandler } from "svelte/elements";
    import type { ConversationOptionDto } from "../../types/dto/conversation/conversationOption";

    export let tabindex: number;
    export let conversationOption: ConversationOptionDto;
    export let isSelected: boolean;
    export let selected: (id: string) => MouseEventHandler<HTMLDivElement> | null | undefined;
    export let deleteConversation: (id: string) => void;
</script>

<div
    role="button"
    class={`grid grid-cols-[20px_1fr] gap-1 ${isSelected ? "bg-zinc-700" : ""} hover:bg-zinc-700 select-none p-0.5 mx-1 rounded-sm`}
    tabindex={tabindex}
    on:click={selected(conversationOption.id)}
    on:keypress={(keypress) => {if (keypress.key === " ") selected(conversationOption.id)}}>

    <div>
        <button
            class="font-mono text-zinc-400 bg-zinc-700 hover:text-white hover:bg-red-700 active:bg-green-700 w-full h-full rounded-sm"
            on:click={() => deleteConversation(conversationOption.id)}>X</button>
    </div>

    <div>
        <h3 class="my-auto h-full text-xs text-ellipsis overflow-hidden">{conversationOption.summary}</h3>
    </div>
</div>