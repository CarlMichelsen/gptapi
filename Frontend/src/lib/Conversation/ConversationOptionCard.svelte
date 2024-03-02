<script lang="ts">
    import type { MouseEventHandler } from "svelte/elements";
    import type { ConversationMetadata } from "../../types/dto/conversation";
    import { displayDate } from "../../util/dateParse";

    export let tabindex: number;
    export let conversationOption: ConversationMetadata;
    export let isSelected: boolean;
    export let selected: (id: string) => MouseEventHandler<HTMLDivElement> | null | undefined;
</script>

<div
    role="button"
    class="grid grid-rows-[h-fit_1fr] bg-gray-500 hover:bg-gray-700 select-none"
    tabindex={tabindex}
    on:click={selected(conversationOption.id)}
    on:keypress={(keypress) => {if (keypress.key === " ") selected(conversationOption.id)}}>

    <h3>{conversationOption.summary}</h3>
    <p class="text-sm">{displayDate(conversationOption.lastAppendedUtc)}</p>
    {#if isSelected}
        <p>SELECTED</p>
    {/if}
</div>