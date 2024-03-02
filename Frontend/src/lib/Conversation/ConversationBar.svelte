<script lang="ts">
    import { applicationStore } from "../../store/applicationStore";
    import ConversationOptionList from "./ConversationOptionList.svelte";
    import CovnersationBarHeader from "./CovnersationBarHeader.svelte";
</script>

{#if $applicationStore.state === "logged-in" && $applicationStore.conversations !== null}
<div class="grid sm:grid-cols-1 sm:grid-rows-[100px_1fr] grid-rows-1 grid-cols-[250px_1fr] h-full gap-2">
    <CovnersationBarHeader />
    <ConversationOptionList conversationOptionList={$applicationStore.conversations} />
</div>
{/if}