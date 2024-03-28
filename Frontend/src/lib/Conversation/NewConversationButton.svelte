<script lang="ts">
    export let text: string;
    export let clicked: () => void;
</script>

<div
    role="button"
    class="grid grid-cols-[35px_1fr_25px] h-[30px] gap-1 hover:bg-zinc-700 select-none p-0.5 mx-1 rounded-sm"
    tabindex="0"
    on:click={() => clicked()}
    on:keypress={(keypress) => {if (keypress.key === " ") clicked()}}>

    <div class="relative">
        <p class="pointer-events-none text-4xl absolute left-0 -top-3">+</p>
    </div>

    <div>
        <h3 class="block mt-1 text-xs text-ellipsis overflow-hidden">{text}</h3>
    </div>

    <div>
        <p>📃</p>
    </div>
</div>