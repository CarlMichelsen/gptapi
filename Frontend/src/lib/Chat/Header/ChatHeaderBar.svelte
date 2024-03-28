<script lang="ts">
    import { applicationStore } from "../../../store/applicationStore";
    import { localStore } from "../../../store/localStore";
    import type { AvailableModel } from "../../../types/dto/availableModel/availableModel";
    import ModelSelector from "../../Interaction/ModelSelector.svelte";

    const selectModel = (model: AvailableModel) => {
        applicationStore.selectAvailableModel(model)

        localStore.update((state) => {
            return {
                ...state,
                preferences: {
                    lastSelectedModel: model,
                }
            }
        });
    }
    
</script>

{#if $applicationStore.state === "logged-in" && $applicationStore.languageModel !== null}
<div>
    <ModelSelector
        languageModel={$applicationStore.languageModel}
        selectModel={(model) => selectModel(model)} />
</div>
{/if}