<script lang="ts">
    import { applicationStore } from "../../../store/applicationStore";

    import { ConnectionMethods } from "../../../connectionMethods";
    import type { Role } from "../../../types/dto/conversation/role";

    export let index: number;
    export let messageId: string;
    export let role: Role;
</script>

{#if $applicationStore.state === "logged-in"}
<div class="grid grid-cols-[25px_100px_1fr] h-[25px] mb-2">
    <div class="relative">
        <div class="absolute top-0 w-[30px] h-[30px]">
            {#if index === -1}
                <button
                    class="absolute top-0 w-full h-full bg-red-700 text-white hover:text-black hover:bg-red-900 font-bold rounded-lg"
                    on:click={() => ConnectionMethods.cancelMessage(messageId)}>X</button>
            {:else}
                {#if role === "user"}
                    <img src={$applicationStore.user.avatarUrl} alt="profile" class="w-full h-full rounded-lg">
                {:else}
                    <div class="absolute top-0 w-full h-full bg-zinc-400 rounded-lg"></div>
                {/if}
            {/if}
        </div>
    </div>

    <div>
        <p class="ml-2 font-bold text-xl">{role === "user" ? "You" : "Assistant"}</p>
    </div>

    <div>
        <p class="font-mono font-thin text-xs text-zinc-600 float-right mt-2">{index} - {messageId}</p>
    </div>
</div>
{/if}