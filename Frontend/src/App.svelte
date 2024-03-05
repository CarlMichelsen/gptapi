<script lang="ts">
    import { applicationStore } from "./store/applicationStore";
  	import Authenticated from "./lib/Authenticated.svelte";
    import LandingZone from "./lib/LandingZone.svelte";

	applicationStore.login();
</script>

<main>
	{#if $applicationStore.ready}
		{#if $applicationStore.state === "logged-in"}
			<Authenticated />
		{:else}
			<LandingZone />
		{/if}
	{:else}
		<div></div>
	{/if}
</main>