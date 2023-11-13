<script lang="ts">
  	import Authenticated from "./lib/Authenticated.svelte";
    import LoginForm from "./lib/LoginForm.svelte";
    import { localStore } from "./store/localStore";
    import { type Credentials } from "./types/localStore";

	const onAuthenticate = (credentials: { detail: Credentials }) => {
		localStore.set({ credentials: credentials.detail });
	}
</script>

<main>
	{#if $localStore.credentials}
  		<Authenticated
			username={$localStore.credentials.username}
			password={$localStore.credentials.password} />
	{:else}
		<div class="pt-12">
			<LoginForm on:authenticate={onAuthenticate} />
		</div>
	{/if}
</main>