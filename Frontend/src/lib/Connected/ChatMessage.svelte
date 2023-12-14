<script lang="ts">
    import { marked, Renderer, type MarkedOptions } from "marked";
    import type { Message } from "../../types/dto/message";
    import { loadLanguage } from "../../util/syntaxHighlight";
    export let message: Message;

    let content: string = "";
    $: content = processContent(message);

    const header = (code: string, language: string | undefined): string => {
        return `<div class="rounded-t-md border-b"><p class="mb-1">${language ?? "text"}</p></div>`;
    }

    const renderer: Renderer = new marked.Renderer();
    renderer.code = (code: string, language: string | undefined): string => {
        language && loadLanguage(language);
        const safeCode = code
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;"); 
        return `<div class="bg-black overflow-x-scroll rounded-md">${header(safeCode, language)}<div class="m-1"><pre><code class="lang-${language}">${safeCode}</code></pre></div></div>`;
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

<div class="rounded-sm px-2 pb-2 hover:outline outline-1">
    <p class="text-sm text-gray-400">{message.role}</p>
    {@html content}
</div>