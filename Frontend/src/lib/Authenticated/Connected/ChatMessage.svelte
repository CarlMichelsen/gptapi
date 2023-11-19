<script lang="ts">
    import type { Message } from "../../../types/dto/message";

    export let message: Message;

    let content: string = "";
    $: content = processContent(message);

    const processContent = (msg: Message): string => {
        return msg.content.replaceAll("\n", "<br />");
    }
</script>

<div class="rounded-sm px-2 pb-2 hover:outline outline-1">
    <p class="text-sm text-gray-400">{message.role}</p>
    <p class="break-all">{@html content}<p/>
</div>