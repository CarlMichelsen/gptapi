<script lang="ts">
    import { userStore } from "./store/userStore";
  	import Authenticated from "./lib/Authenticated/index.svelte";
    import LandingZone from "./lib/LandingZone.svelte";

	userStore.login();
</script>

<main>
	{#if $userStore?.user != null}
  		<Authenticated />
	{:else}
		<LandingZone />
	{/if}
</main>