<script lang="ts">
    import { ConnectionMethods } from "../../connectionMethods";
    import type { Role } from "../../types/dto/role";

    export let index: number;
    export let messageId: string;
    export let role: Role;
</script>

<div class="grid grid-cols-[20px_1fr_270px]">
    <div>
        {#if index === -1}
            <button on:click={() => ConnectionMethods.cancelMessage(messageId)}>X</button>
        {:else}
            <p class="text-xs text-gray-400 my-auto">{index}</p>
        {/if}
    </div>

    <div>
        <p class="text-xs text-gray-400 my-auto">{role}</p>
    </div>
    
    <div>
        <code class="text-xs text-gray-400 float-right my-auto">{messageId}</code>
    </div>
</div>