<script lang="ts">
    import { marked } from "marked";

    const renderer = new marked.Renderer();
    renderer.code = (code: string, infostring: string | undefined, escaped: boolean): string => {
        return `<pre class="my-1 overflow-auto bg-black p-0.5"><code class="${infostring ? `language-${infostring}` : ''}">${code}</code></pre>`;
    }

    marked.use({
        renderer: renderer
    });

    const parse = (content: string): string => {
        return marked.parse(content.replace(/^[\u200B\u200C\u200D\u200E\u200F\uFEFF]/,"")) as string;
    }

    export let content: string;
    let parsedContent: string = parse(content);

    $: parsedContent = parse(content);
</script>

<p>{@html parsedContent}</p>