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
    class={`hover:bg-zinc-700 ${isSelected ? "bg-zinc-700" : ""} select-none p-0.5 mx-1 rounded-sm group`}
    tabindex={tabindex}
    on:click={selected(conversationOption.id)}
    on:keypress={(keypress) => {if (keypress.key === " ") selected(conversationOption.id)}}>

    <div class="relative">
        <h3 class="my-auto h-full text-xs text-ellipsis overflow-hidden py-1 pr-3">{conversationOption.summary}</h3>
        <button
            on:click={() => deleteConversation(conversationOption.id)}
            class="absolute right-0 top-0 h-full invisible group-hover:visible"
            id={"option-"+conversationOption.id}>
            <div class="grid grid-rows-3 h-6 w-3 py-1 my-auto rounded-sm hover:bg-zinc-400">
                <div class="bg-zinc-200 rounded-full w-1 h-1 m-auto"></div>
                <div class="bg-zinc-200 rounded-full w-1 h-1 m-auto"></div>
                <div class="bg-zinc-200 rounded-full w-1 h-1 m-auto"></div>
            </div>
        </button>
    </div>
</div>