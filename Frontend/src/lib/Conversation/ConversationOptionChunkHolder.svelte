<script lang="ts">
    import ConversationOptionCard from "./ConversationOptionCard.svelte"
    import type { MouseEventHandler } from "svelte/elements";
    import type { ConversationOptionDateChunk } from "../../types/dto/conversationOption";
    import { applicationStore } from "../../store/applicationStore";

    export let conversationOptionDateChunk: ConversationOptionDateChunk;

    const selected = (id: string): MouseEventHandler<HTMLDivElement> | null | undefined => {
        applicationStore.selectConversation(id);
        return null;
    }


    const deleteConversation = (id: string): void => {
        applicationStore.deleteConversation(id);
    }
</script>
{#if $applicationStore.state === "logged-in"}
<div>
    <h5 class="mx-2 underline">{conversationOptionDateChunk.dateText}</h5>
    <ol class="space-y-1">
        {#each conversationOptionDateChunk.options as conversationOption, index}
        <li>
            <ConversationOptionCard
                conversationOption={conversationOption}
                selected={selected}
                deleteConversation={deleteConversation}
                isSelected={$applicationStore.selectedConversation?.id === conversationOption.id}
                tabindex={index} />
        </li>
        {/each}
    </ol>
</div>
{/if}

