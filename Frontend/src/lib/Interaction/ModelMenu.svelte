<script lang="ts">
    import type { AvailableModel } from "../../types/dto/availableModel/availableModel";
    import type { LanguageModel } from "../../types/store/applicationStore";

    export let languageModel: LanguageModel;
    export let selectModel: (model: AvailableModel) =>  void;

    const providerModels = Object.keys(languageModel.availableModels).map((key) => {
        return {
            provider: key,
            models: languageModel.availableModels[key]!,
        }
    });
</script>

<div class="w-96 p-2 rounded-md bg-zinc-700">
    <ol class="gap-4">
    {#each providerModels as providerModel}
        <li>
            <p>{providerModel.provider}</p>
            <ol class="gap-2">
            {#each providerModel.models as model}
                <li>
                    <button
                        class="px-2 ml-4 hover:bg-zinc-950"
                        on:click={() => selectModel(model)}>{model.displayName}</button>
                </li>
            {/each}
            </ol>
        </li>
    {/each}
    </ol>
</div>