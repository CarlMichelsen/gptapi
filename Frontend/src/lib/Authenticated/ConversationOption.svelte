<script lang="ts">
    import type { ConversationMetadata } from "../../types/dto/conversation";

    export let metaData: ConversationMetadata;
    export let onSelected: (metaData: ConversationMetadata) => void;
</script>

<button class="px-2 py-2 w-full bg-zinc-800 hover:bg-black hover:underline" on:click={() => onSelected(metaData)}>
    <p>{metaData.title}</p>
</button>