<script lang="ts">
    export let isMessage: boolean;
    export let id: string;
</script>

<div
    id={id}
    class={`mx-auto w-full md:w-[500px] lg:w-[750px] xl:w-[900px] ${isMessage ? "border-x border-zinc-500 hover:border-zinc-400 px-1" : ""}`}>
    <slot />
</div>