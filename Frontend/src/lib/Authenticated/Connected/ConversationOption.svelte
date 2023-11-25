<script lang="ts">
    import type { ConversationMetadata } from "../../../types/dto/conversation";

    export let isSelected: boolean;
    export let metaData: ConversationMetadata;
    export let onSelected: (metaData: ConversationMetadata) => void;
    export let onDelete: (metaData: ConversationMetadata) => void;
</script>

<div class={`${isSelected ? "pl-1" : "mr-1"} grid grid-cols-8`}>
    <div class="mr-1">
        <button class="w-full h-full text-red-700 text-xl bg-zinc-800 hover:bg-red-600 hover:text-white outline-none border-none cursor-pointer" on:click={() => onDelete(metaData)}>X</button>
    </div>
    <div class="col-span-7 bg-zinc-800 hover:bg-zinc-600 text-zinc-300 p-1 h-24">
        <button class="w-full h-full bg-transparent outline-none border-none cursor-pointer" on:click={() => {
            if (!isSelected) onSelected(metaData);
        }}>{metaData?.summary ? metaData.summary : "..."}</button>
    </div>
</div>