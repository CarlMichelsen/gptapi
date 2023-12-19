<script lang="ts">
    import type { ConversationMetadata } from "../../types/dto/conversation";

    export let isSelected: boolean;
    export let metaData: ConversationMetadata;
    export let onSelected: (metaData: ConversationMetadata) => void;
    export let onDelete: (metaData: ConversationMetadata) => void;
</script>

<div class="flex w-[20rem]">
    <button
        on:click={() => onDelete(metaData)}
        class="flex-none w-6 h-12 bg-zinc-800 hover:bg-red-600 hover:underline">X</button>
    <button
        on:click={() => onSelected(metaData)}
        class={`flex-1 text-sm text-left pl-1 h-12 mx-1 bg-zinc-800 hover:bg-green-600 hover:underline ${isSelected && "underline"}`}>{metaData.summary}</button>
</div>