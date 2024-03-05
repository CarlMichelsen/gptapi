<script lang="ts">
    import { applicationStore } from "../../store/applicationStore";

    const createNewConversation = () => {
        applicationStore.selectConversation(null);
    }
</script>

{#if $applicationStore.state === "logged-in"}
<div class="grid grid-cols-[100px_1fr] px-1 gap-1">
    <div>
        <picture>
            <source src={$applicationStore.user.avatarUrl} />
            <source src="https://placehold.co/100x100/000000/white.jpg">
            <img src="https://placehold.co/100x100/000000/white.jpg" alt="profile" class="rounded-md w-[100px] h-[100px]" />
        </picture>
    </div>
    
    <div class="grid sm:grid-rows-[25px_1fr] grid-rows-[50px_1fr]">
        <div>
            <p>{$applicationStore.user.name}</p>
        </div>
        <div class="pt-1">
            <button class="w-full h-full relative hover:bg-green-900 bg-zinc-700 rounded-md" on:click={createNewConversation}>
                <p class="absolute w-full text-tcenter sm:-top-6 sm:left-0 sm:text-8xl -top-3.5 left-0 text-6xl">+</p>
            </button>
        </div>
    </div>
</div>
{/if}