<script lang="ts">
    export let className: string;
    export let id: string;
</script>

<div class={`${className} w-full md:w-[29em] lg:w-[45em] xl:w-[61em] 2xl:w-[77em]`} id={id}>
    <slot></slot>
</div>