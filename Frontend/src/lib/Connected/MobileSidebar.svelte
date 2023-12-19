<script lang="ts">
    import MobileConversationOption from "./MobileConversationOption.svelte";
    import type { ConversationMetadata } from "../../types/dto/conversation";
    import { applicationStore } from "../../store/applicationStore";

    let expanded = false;

    const dynamicHeight = (expanded: boolean): string => {
        return expanded ? "absolute h-96" : "h-full";
    }

    const onSelected = async (metaData: ConversationMetadata|null) => {
        await applicationStore.selectConversation(metaData?.id ?? null);
        expanded = false;
    }

    const onDelete = async (metaData: ConversationMetadata) => {
        await applicationStore.deleteConversation(metaData.id);
    };
</script>

{#if $applicationStore.user}
<div class="relative h-full">
    <div class={`${dynamicHeight(expanded)} bg-black w-full transition-all ease-in-out duration-100 relative`}>
        <button
            on:click={() => expanded = !expanded}
            class={`absolute right-2.5 -bottom-1 font-mono font-extrabold text-4xl p-2 transition-transform ease-in-out duration-100 hover:text-green-400 ${expanded ? "-rotate-90" : "rotate-90"}`}>></button>

        <div>
            <h1 class="ml-2">Conversations of {$applicationStore.user?.name}</h1>
            <div class="w-72 grid grid-cols-2">
                <button
                    class="text-sm text-left px-1 mx-1 bg-zinc-800 hover:bg-red-600 hover:underline"
                    on:click={() => applicationStore.logout()}>Log out</button>
                <button
                    class="text-sm text-left px-1 mx-1 bg-zinc-800 hover:bg-green-600 hover:underline"
                    on:click={() => onSelected(null)}>New Conversation</button>
            </div>
        </div>

        <div class={`${expanded ? "block" : "hidden"}`}>
            <hr class="mt-1">
            <ol class="overflow-y-scroll">
                {#if $applicationStore.conversations === null}
                    <p>Loading conversations...</p>
                {:else}
                    {#each $applicationStore.conversations as conv}
                        <li>
                            <MobileConversationOption metaData={conv} {onSelected} {onDelete} isSelected={(conv.id === $applicationStore.selectedConversation?.id)} expanded={expanded} />
                        </li>
                    {/each}
                {/if}
            </ol>
        </div>
        
    </div>
</div>
{/if}