<script lang="ts">
    import ConversationOptionChunkHolder from "./ConversationOptionChunkHolder.svelte";
    import type { ConversationOptionDateChunk } from "../../types/dto/conversation/conversationOption";
    import NewConversationButton from "./NewConversationButton.svelte";
    import { applicationStore } from "../../store/applicationStore";

    export let conversationOptionList: ConversationOptionDateChunk[];
</script>


<div class="relative">
    <div class="absolute w-[250px] top-3">
        <NewConversationButton text="New chat" clicked={() => applicationStore.selectConversation(null)} />
    </div>

    <ol class="space-y-5 h-full pt-12">
        {#each conversationOptionList as conversationOptionDateChunk}
            <li>
                <ConversationOptionChunkHolder conversationOptionDateChunk={conversationOptionDateChunk} />
            </li>
        {/each}
    </ol>
</div>

