<script lang="ts">
    import { applicationStore } from "../../../store/applicationStore";
    import ModelSelector from "../../Interaction/ModelSelector.svelte";


</script>

{#if $applicationStore.state === "logged-in" && $applicationStore.languageModel !== null}
<div>
    <ModelSelector
        languageModel={$applicationStore.languageModel}
        selectModel={(model) => applicationStore.selectAvailableModel(model)} />
</div>
{/if}