<script lang="ts">
    import { createEventDispatcher } from "svelte";

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

<div class="flex flex-row items-center container mb-8 mt-4">
    <input
        type="text"
        bind:value={inputValue}
        on:keydown={handleKeyDown}
        placeholder={placeholder}
        class="flex-1 p-3 text-base leading-tight bg-gray-700 border-none rounded focus:outline-none focus:ring-2 focus:gray-700"
    />
    <button
        on:click={handleOnSend}
        class="ml-2 bg-green-800 hover:bg-green-800 focus:outline-none focus:ring-2 focus:ring-green-600 focus:ring-opacity-50 rounded-md px-4 py-3"
        disabled={readyToSend()}
    >
        Send
    </button>
</div>