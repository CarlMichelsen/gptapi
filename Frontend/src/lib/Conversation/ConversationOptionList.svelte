<script lang="ts">
    import ConversationOptionChunkHolder from "./ConversationOptionChunkHolder.svelte";
    import type { ConversationOptionDateChunk } from "../../types/dto/conversationOption";

    export let conversationOptionList: ConversationOptionDateChunk[]
</script>

<li>
    <ol class="space-y-4 h-full">
    {#each conversationOptionList as conversationOptionDateChunk}
        <li>
            <ConversationOptionChunkHolder conversationOptionDateChunk={conversationOptionDateChunk} />
        </li>
    {/each}
    </ol>
</li>