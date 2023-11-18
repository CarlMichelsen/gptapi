<script lang="ts">
    import ConversationOption from "./ConversationOption.svelte";
    import type { ConversationMetadata } from "../../types/dto/conversation";
    import { userStore } from "../../store/userStore";

    export let conversationOptions: ConversationMetadata[];
    export let onNewConversation: () => void;

    const onSelected = (metaData: ConversationMetadata) => {
        console.log("selected", metaData);
    }
</script>

<div class="w-full h-full">
    <div class="fixed left-0 bg-black h-full w-[200px]">
        <div class="grid grid-cols-[140px_minmax(0,1fr)]">
            <div class="col-span-2">
                <button
                    class="w-full h-9"
                    on:click={() => userStore.logout()}>
                    Logout
                </button>
            </div>

            <div>
                <h1 class="text-xl my-2.5 ml-1">Conversations</h1>
            </div>
            
            <div>
                <button
                    on:click={onNewConversation}
                    class="font-extrabold text-4xl text-green-600 hover:text-green-400 block w-full h-full pb-1">+</button>
            </div>
        </div>
        

        <ol class="space-y-1 p-1">
            {#each conversationOptions as option}
                <li>
                    <ConversationOption metaData={option} {onSelected} />
                </li>
            {/each}
        </ol>
    </div>
</div>