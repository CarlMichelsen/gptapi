<script lang="ts">
    import type { AvailableModel } from "../../types/dto/availableModel/availableModel";
    import type { LanguageModel } from "../../types/store/applicationStore";
    import ModelMenu from "./ModelMenu.svelte"

    export let languageModel: LanguageModel | null;
    export let selectModel: (model: AvailableModel) =>  void;

    let menuOpen: boolean = false;

    const click = () => {
        menuOpen = !menuOpen
    }
</script>

<div
    role="button"
    tabindex="0"
    on:click={click}
    on:keypress={(keypress) => {if (keypress.key === " ") click()}}
    class="relative p-2 cursor-pointer hover:bg-zinc-700 w-64 grid grid-cols-[1fr_20px]">

    <h3>{languageModel?.selectedModel.displayName}</h3>

    <div>
        <p class="rotate-90 font-mono font-bold text-xl overflow-hidden">⪼</p>
        {#if menuOpen && languageModel !== null}
            <div class="absolute left-2 md:left-24 top-14 cursor-default">
                <ModelMenu languageModel={languageModel} selectModel={selectModel} />
            </div>
        {/if}
    </div>
</div>