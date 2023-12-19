<script lang="ts">
    import type { ConversationMetadata } from "../../types/dto/conversation";

    export let expanded: boolean;
    export let isSelected: boolean;
    export let metaData: ConversationMetadata;
    export let onSelected: (metaData: ConversationMetadata) => void;
    export let onDelete: (metaData: ConversationMetadata) => void;
</script>

<div>
    <button on:click={() => onSelected(metaData)}>{metaData.summary}</button>
</div>