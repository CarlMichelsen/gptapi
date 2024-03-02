<script lang="ts">
    export let index: number;
    export let messageId: string;
    export let dateText: string;
</script>

<div class="grid grid-cols-[250px_1fr]">
    <p class="text-xs text-gray-400 my-auto">{index} - {messageId}</p>
    <p class="text-xs text-gray-400 my-auto">{dateText}</p>
</div>