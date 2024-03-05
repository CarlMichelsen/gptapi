<script lang="ts">
    import ConversationOptionChunkHolder from "./ConversationOptionChunkHolder.svelte";
    import type { ConversationOptionDateChunk } from "../../types/dto/conversationOption";

    export let conversationOptionList: ConversationOptionDateChunk[]
</script>

<div>
    <ol class="space-y-4 overflow-y-scroll no-scrollbar h-full">
    {#each conversationOptionList as conversationOptionDateChunk}
        <li>
            <ConversationOptionChunkHolder conversationOptionDateChunk={conversationOptionDateChunk} />
        </li>
    {/each}
    </ol>
</div>