<script lang="ts">
    import ConversationOptionCard from "./ConversationOptionCard.svelte"
    import type { MouseEventHandler } from "svelte/elements";
    import { DateRange, type ConversationOptionDateChunk } from "../../types/dto/conversation/conversationOption";
    import { applicationStore } from "../../store/applicationStore";
    import { ConversationClient } from "../../clients/conversationClient";

    export let conversationOptionDateChunk: ConversationOptionDateChunk;

    const attemptSelectConversation = async (id: string) => {
        const conversationClient = new ConversationClient();
        const response = await conversationClient.getConversation(id);
        if (response.ok) {
            applicationStore.selectConversation(response.data);
        }
    }

    const selected = (id: string): MouseEventHandler<HTMLDivElement> | null | undefined => {
        attemptSelectConversation(id);
        return null;
    }

    const deleteConversation = async (id: string): Promise<void> => {
        const conversationClient = new ConversationClient();
        const deleteResponse = await conversationClient.deleteConversation(id);
        if (deleteResponse.ok && deleteResponse.data) {
            applicationStore.deleteConversation(id);
        }
    }

    const mapDateRange = (dateRange: DateRange): string => {
        switch (dateRange) {
            case DateRange.today:
                return "Today";
            
            case DateRange.yesterday:
                return "Yesterday";
            
            case DateRange.week:
                return "A week ago";

            case DateRange.month:
                return "A month ago";

            case DateRange.year:
                return "A year ago";
            
            case DateRange.unknown:
                return "Unknown";

            default:
                return "Unknown...";
        }
    }
</script>
{#if $applicationStore.state === "logged-in"}
<div>
    <h5 class="mx-2 underline">{mapDateRange(conversationOptionDateChunk.dateRange)}</h5>
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

