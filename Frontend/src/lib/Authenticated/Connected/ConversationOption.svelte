<script lang="ts">
    import type { ConversationMetadata } from "../../../types/dto/conversation";

    export let isSelected: boolean;
    export let metaData: ConversationMetadata;
    export let onSelected: (metaData: ConversationMetadata) => void;
</script>

<div class={isSelected ? "" : "mr-1"}>
    <button class="px-2 py-2 w-full bg-zinc-800 hover:bg-zinc-600 hover:underline" on:click={() => onSelected(metaData)}>
        {metaData.summary}
    </button>
</div>