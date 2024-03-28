<script lang="ts">
    export let isMessage: boolean;
    export let id: string;
</script>

<div
    id={id}
    data-ismessage={isMessage}
    class={`mx-auto w-full md:w-[500px] lg:w-[750px] xl:w-[900px]`}>
    <slot />
</div>