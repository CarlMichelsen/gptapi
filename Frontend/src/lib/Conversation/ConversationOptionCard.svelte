<script lang="ts">
    import type { MouseEventHandler } from "svelte/elements";
    import type { ConversationMetadata } from "../../types/dto/conversation";

    export let tabindex: number;
    export let conversationOption: ConversationMetadata;
    export let isSelected: boolean;
    export let selected: (id: string) => MouseEventHandler<HTMLDivElement> | null | undefined;
</script>

<div
    role="button"
    class={`grid grid-rows-[h-fit_1fr] ${isSelected ? "bg-zinc-700" : ""} hover:bg-zinc-700 select-none p-1 mx-1 rounded-sm`}
    tabindex={tabindex}
    on:click={selected(conversationOption.id)}
    on:keypress={(keypress) => {if (keypress.key === " ") selected(conversationOption.id)}}>

    <h3>{conversationOption.summary}</h3>
</div>