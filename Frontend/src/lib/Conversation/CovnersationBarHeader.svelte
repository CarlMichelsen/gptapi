<script lang="ts">
    import { applicationStore } from "../../store/applicationStore";
</script>

{#if $applicationStore.state === "logged-in"}
<div class="grid grid-cols-[100px_1fr] px-1 gap-1">
    <div>
        <img src={$applicationStore.user.avatarUrl} alt="profile" class="rounded-md w-[100px] h-[100px]" />
    </div>
    
    <div class="grid sm:grid-rows-[25px_1fr] grid-rows-[50px_1fr]">
        <div>
            <p>{$applicationStore.user.name}</p>
        </div>
    </div>
</div>
{/if}