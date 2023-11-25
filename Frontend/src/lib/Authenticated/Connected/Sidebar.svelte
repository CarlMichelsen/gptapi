<script lang="ts">
    import ConversationOption from "./ConversationOption.svelte";
    import type { ConversationMetadata } from "../../../types/dto/conversation";
    import { userStore } from "../../../store/userStore";
    import { createEventDispatcher } from "svelte";
    import { deleteConversation } from "../../../clients/conversationClient";
    import { setQueryParam } from "../../../util/queryParameters";

    export let conversationOptions: ConversationMetadata[] | null;
    export let selectedConversationId: string | null
    
    const dispatch = createEventDispatcher<{ conversationSelected: string|null }>();

    const onSelected = (metaData: ConversationMetadata|null) => dispatch("conversationSelected", metaData?.id);
    const onDelete = async (metaData: ConversationMetadata) => {
        const deleted = await deleteConversation(metaData.id)
        if (deleted) {
            const index = conversationOptions?.findIndex(c => c.id === metaData.id) ?? -1;
            if (index !== -1) {
                conversationOptions!.splice(index, 1);
                conversationOptions = [ ...conversationOptions! ];
                if (selectedConversationId === metaData.id) {
                    selectedConversationId = null;
                    setQueryParam("conv", null);
                }
            }
        }
    };
</script>

<div class="w-full h-full">
    <div class="left-0 bg-black h-screen grid grid-rows-[auto,auto,1fr] overflow-y-scroll">
        <div>
            <h1 class="text-xl my-2.5 ml-1 text-zinc-300">Conversations of {$userStore.user?.personaname}</h1>
        </div>

        <div class="grid grid-cols-2 space-x-1 mx-1 mb-4">
            <button class="bg-zinc-800 hover:bg-red-600 hover:text-white py-3" on:click={() => userStore.logout()}>Logout</button>
            <button class="bg-zinc-800 hover:bg-green-600 hover:text-white py-3" on:click={() => onSelected(null)}>New Conversation</button>
        </div>
        
        <div>
            <ol class="space-y-1 pl-1 py-1">
                {#if conversationOptions == null}
                    <p>Loading conversations...</p>
                {:else}
                    {#each conversationOptions as option}
                        <li>
                            <ConversationOption metaData={option} {onSelected} {onDelete} isSelected={(option.id === selectedConversationId)} />
                        </li>
                    {/each}
                {/if}
            </ol>
        </div>
    </div>
</div>