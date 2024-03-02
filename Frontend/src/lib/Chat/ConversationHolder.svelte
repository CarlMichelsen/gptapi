<script lang="ts">
    import { applicationStore } from "../../store/applicationStore";
    import ChatOrchestrator from "./ChatOrchestrator.svelte";


</script>

{#if $applicationStore.state === "logged-in"}
<ChatOrchestrator />
{/if}