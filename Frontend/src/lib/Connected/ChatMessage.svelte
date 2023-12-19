<script lang="ts">
    import { marked, Renderer, type MarkedOptions } from "marked";
    import type { Message } from "../../types/dto/message";
    import { loadLanguage } from "../../util/syntaxHighlight";
    export let message: Message;

    let content: string = "";
    $: content = processContent(message);

    const removeUnsafeTags = (unsafe: string): string => {
        return unsafe.replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    const header = (code: string, language: string | undefined): string => {
        return `<div class="rounded-t-md border-b"><p class="mb-1">${language ?? "text"}</p></div>`;
    }

    const renderer: Renderer = new marked.Renderer();
    renderer.code = (code: string, language: string | undefined): string => {
        language && loadLanguage(language);
        const safeCode = removeUnsafeTags(code); 
        return `<div class="bg-black overflow-x-scroll rounded-md">${header(safeCode, language)}<div class="m-1"><code class="lang-${language}">${safeCode}</code></div></div>`;
    };

    const options: MarkedOptions = {
        gfm: true,
        breaks: true,
        renderer,
    };

    const processContent = (msg: Message): string => {
        return msg.role === "assistant" ? marked(msg.content, options) : msg.content;
    }
</script>

<div class="rounded-sm px-2 pb-2 outline-1">
    <p class="text-sm text-gray-400">{message.role}</p>
    <div class="overflow-x-scroll">
        {#if message.role === "assistant"}
            <pre class="font-sans">{@html content}</pre>
        {:else}
            <pre class="font-sans">{content}</pre>
        {/if}
    </div>
</div>