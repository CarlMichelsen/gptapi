<script lang="ts">
    import { onDestroy } from "svelte";
    export let className: string;

    let rotation = 45;
    const rotate = () => {
        rotation += 3;
        rotation = rotation % 360;
    }

    let intervalId = setInterval(() => rotate(), 25);
    onDestroy(() => intervalId && clearInterval(intervalId));
</script>

<div class={`w-32 h-32 relative ${className}`}>
    <div class="absolute w-32 h-32 border border-white rounded-2xl" style="transform: rotate({rotation}deg)"></div>
</div>