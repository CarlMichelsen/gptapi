<script lang="ts">
    import { applicationStore } from "../../store/applicationStore";
    import ConversationOptionList from "./ConversationOptionList.svelte";
    import CovnersationBarHeader from "./CovnersationBarHeader.svelte";
</script>

{#if $applicationStore.state === "logged-in" && $applicationStore.conversationChunks !== null}
<div class="h-screen gap-2 grid grid-rows-[1fr_100px] bg-zinc-950">
    <div class="shadow-inner overflow-y-scroll no-scrollbar">
        <ConversationOptionList conversationOptionList={$applicationStore.conversationChunks} />
    </div>
    <div class="shadow-inner">
        <CovnersationBarHeader />
    </div>
</div>
{/if}