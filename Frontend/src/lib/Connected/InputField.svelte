<script lang="ts">
    import { createEventDispatcher } from "svelte";
    import ConversationContainer from "./ConversationContainer.svelte";

    export let ready: boolean;
    export let placeholder: string = 'Type your message here...';

    const dispatch = createEventDispatcher<{ send: string }>();

    let inputValue: string = '';

    const readyToSend = (): boolean => {
        return inputValue.trim() !== '' && ready;
    }

    const handleOnSend = () => {
        if (readyToSend()) {
            dispatch("send", inputValue);
            inputValue = '';
        }
    }

    const handleKeyDown = (event: KeyboardEvent) => {
        if (event.key === 'Enter' && !event.shiftKey) {
            event.preventDefault();
            handleOnSend();
        }
    };
</script>

<ConversationContainer className="flex flex-row items-center mx-auto" id="input-field">
    <textarea
        contenteditable="true"
        bind:value={inputValue}
        on:keydown={handleKeyDown}
        placeholder={placeholder}
        class="flex-1 p-3 text-base leading-tight bg-gray-700 border-none rounded-tl-md focus:outline-none focus:ring-2 focus:gray-700 resize-none h-32"
    />
    <button
        on:click={handleOnSend}
        class="bg-green-800 hover:bg-green-800 focus:outline-none focus:ring-2 focus:ring-green-600 focus:ring-opacity-50 rounded-tr-md px-4 h-32"
        disabled={readyToSend()}
    >
        Send
    </button>
</ConversationContainer>