<script lang="ts">
    import ConversationOption from "./ConversationOption.svelte";
    import type { ConversationMetadata } from "../../../types/dto/conversation";
    import { userStore } from "../../../store/userStore";
    import { createEventDispatcher } from "svelte";

    export let conversationOptions: ConversationMetadata[] | null;
    export let selectedConversationId: string | null
    
    const dispatch = createEventDispatcher<{ conversationSelected: string }>();

    const onSelected = (metaData: ConversationMetadata) => dispatch("conversationSelected", metaData.id);
</script>

<div class="w-full h-full">
    <div class="fixed left-0 bg-black h-full w-[200px]">
        <div class="grid grid-cols-[140px_minmax(0,1fr)]">
            <div>
                <h1 class="text-xl my-2.5 ml-1">Conversations</h1>
            </div>
            
            <div>
                <button
                    on:click={() => userStore.logout()}
                    class="font-extrabold text-4xl text-red-600 hover:text-red-400 block w-full h-full pb-1">-</button>
            </div>
        </div>
        
        <ol class="space-y-1 pl-1 py-1">
            {#if conversationOptions == null}
                <p>Loading conversations...</p>
            {:else}
                {#each conversationOptions as option}
                    <li>
                        <ConversationOption metaData={option} {onSelected} isSelected={(option.id === selectedConversationId)} />
                    </li>
                {/each}
            {/if}
        </ol>
    </div>
</div>