<script lang="ts">
    import ConversationOption from "./ConversationOption.svelte";
    import type { ConversationMetadata } from "../../types/dto/conversation";
    import { applicationStore } from "../../store/applicationStore";

    const onSelected = async (metaData: ConversationMetadata|null) => {
        await applicationStore.selectConversation(metaData?.id ?? null);
    }

    const onDelete = async (metaData: ConversationMetadata) => {
        await applicationStore.deleteConversation(metaData.id);
    };
</script>

{#if $applicationStore.user}
<div class="w-full h-full">
    <div class="left-0 bg-black h-screen grid grid-rows-[auto,auto,1fr] overflow-y-scroll">
        <div>
            <h1 class="text-xl my-2.5 ml-1 text-zinc-300">Conversations of {$applicationStore.user.personaname}</h1>
        </div>

        <div class="grid grid-cols-2 space-x-1 mx-1 mb-4">
            <button class="bg-zinc-800 hover:bg-red-600 hover:text-white py-3" on:click={() => applicationStore.logout()}>Logout</button>
            <button class="bg-zinc-800 hover:bg-green-600 hover:text-white py-3" on:click={() => onSelected(null)}>New Conversation</button>
        </div>
        
        <div>
            <ol class="space-y-1 pl-1 py-1">
                {#if $applicationStore.conversations === null}
                    <p>Loading conversations...</p>
                {:else}
                    {#each $applicationStore.conversations as conv}
                        <li>
                            <ConversationOption metaData={conv} {onSelected} {onDelete} isSelected={(conv.id === $applicationStore.selectedConversation?.id)} />
                        </li>
                    {/each}
                {/if}
            </ol>
        </div>
    </div>
</div>
{/if}