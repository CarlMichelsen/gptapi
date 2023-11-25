import hljs from 'highlight.js/lib/core';

export const loadLanguage = async (language: string) => {
    // Check if the language is already registered
    if (!hljs.getLanguage(language)) {
        try {
            const module = await importLanguage(language);
            if (module === null) {
                console.error("Count not find", language, "syntax highlighting");
                return;
            }
            hljs.registerLanguage(language, module.default);

            // Now you can highlight code blocks with the newly loaded language
            document.querySelectorAll(`code.lang-${language}`)
                .forEach((block) => hljs.highlightElement(block as HTMLElement));
        } catch (error) {
            console.error(`The language "${language}" could not be loaded`, error);
        }
    }
}

// this is disgusting but vite likes it :(
const importLanguage = async (lang: string): Promise<typeof import("highlight.js/lib/languages/*") | null> => {
    switch (lang) {
        case "javascript":
            return await import("highlight.js/lib/languages/javascript");
        case "typescript":
            return await import("highlight.js/lib/languages/typescript");
        case "python":
            return await import("highlight.js/lib/languages/python");
        case "java":
            return await import("highlight.js/lib/languages/java");
        case "c":
            return await import("highlight.js/lib/languages/c");
        case "cpp":
            return await import("highlight.js/lib/languages/cpp");
        case "ruby":
            return await import("highlight.js/lib/languages/ruby");
        case "go":
            return await import("highlight.js/lib/languages/go");
        case "swift":
            return await import("highlight.js/lib/languages/swift");
        case "php":
            return await import("highlight.js/lib/languages/php");
        case "kotlin":
            return await import("highlight.js/lib/languages/kotlin");
        case "rust":
            return await import("highlight.js/lib/languages/rust");
        case "bash":
            return await import("highlight.js/lib/languages/bash");
        default:
            return null;
    }

}