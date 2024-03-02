<script lang="ts">
    import type { MouseEventHandler } from "svelte/elements";
    import type { ConversationMetadata } from "../../types/dto/conversation";
    import ConversationOptionCard from "../Conversation/ConversationOptionCard.svelte";
    import { applicationStore } from "../../store/applicationStore";

    export let conversationOptionList: ConversationMetadata[]

    const selected = (id: string): MouseEventHandler<HTMLDivElement> | null | undefined => {
        applicationStore.selectConversation(id);
        return null;
    }
</script>

{#if $applicationStore.state === "logged-in"}
<ol class="space-y-1">
{#each conversationOptionList as conversationOption, index}
    <li>
        <ConversationOptionCard
            conversationOption={conversationOption}
            selected={selected}
            isSelected={$applicationStore.selectedConversation?.id === conversationOption.id}
            tabindex={index} />
    </li>
{/each}
</ol>
{/if}