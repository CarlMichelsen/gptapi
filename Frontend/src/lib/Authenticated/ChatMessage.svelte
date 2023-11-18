<script lang="ts">
    import type { Message } from "../../types/dto/message";

    export let message: Message;
</script>

<div class="rounded-sm px-2 pb-2 hover:outline outline-1">
    <p class="text-sm text-gray-400">{message.role}</p>
    <p>{message.content}</p>
</div>