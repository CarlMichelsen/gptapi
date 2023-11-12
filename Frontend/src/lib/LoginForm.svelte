<script lang="ts">
    import { createEventDispatcher } from "svelte";
    import type { Credentials } from "../types/localStore";

    let username: string = "";
    let password: string = "";
    let passwordInput: HTMLInputElement;

    const dispatch = createEventDispatcher();

    const onAuthenticate = () => dispatch("authenticate", { username, password } as Credentials);

    // Function to handle key down events on the username input
    const handleUsernameKeyDown = (event: KeyboardEvent) => {
        if (event.key === 'Enter') {
            passwordInput.focus(); // Move focus to the password input
        }
    };

    // Function to handle key down events on the password input
    const handlePasswordKeyDown = (event: KeyboardEvent) => {
        if (event.key === 'Enter') {
            onAuthenticate(); // Trigger authentication
        }
    };
</script>

<div class="bg-gray-700 rounded-md p-0 pt-1 w-64 mx-auto">
    <input 
        class="mb-0.5 w-60 m-2 h-10 text-lg rounded-sm"
        placeholder="username"
        type="text"
        name="username"
        id="username-input"
        bind:value={username}
        on:keydown={handleUsernameKeyDown} />

    <input 
        class="mt-0.5 w-60 m-2 h-10 text-lg rounded-sm"
        placeholder="password"
        type="password"
        name="password"
        id="password-input"
        bind:value={password}
        on:keydown={handlePasswordKeyDown}
        bind:this={passwordInput} />
    
    <input
        class="w-full h-8 bg-green-800 hover:bg-green-600 active:bg-blue-600 font-bold texg-lg hover:underline text-center cursor-pointer rounded-b-md"
        type="submit"
        value="Authenticate"
        on:click={onAuthenticate} />
</div>