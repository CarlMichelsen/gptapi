<script lang="ts">
    import type { Role } from "../../types/dto/role";

    export let index: number;
    export let messageId: string;
    export let role: Role;
</script>

<div class="grid grid-cols-[20px_1fr_250px]">
    <div>
        <p class="text-xs text-gray-400 my-auto">{index}</p>
    </div>

    <div>
        <p class="text-xs text-gray-400 my-auto">{role}</p>
    </div>
    
    <div>
        <p class="text-xs text-gray-400 float-right my-auto">{messageId}</p>
    </div>
</div>