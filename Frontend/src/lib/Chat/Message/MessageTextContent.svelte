<script lang="ts">
    import type { Message } from "../../../types/dto/conversation/message";
    import AssistantResponseParser from "../AssistantResponseParser.svelte";

    export let message: Message;
</script>

{#if message.role === "assistant"}
    <AssistantResponseParser content={message.content} />
{:else}
    <pre class="font-sans w-full overflow-auto whitespace-break-spaces">{message.content}</pre>
{/if}