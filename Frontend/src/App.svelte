<script lang="ts">
  	import Authenticated from "./lib/Authenticated.svelte";
    import LandingZone from "./lib/LandingZone.svelte";

	export let assumeLoggedIn = false;

	const handleLoginAttempt = () => {
		const url = new URL(window.location.toString());
		const login = url.searchParams.get("login") === "true";
		if (login) {
			window.history.pushState({}, '', url);
			assumeLoggedIn = true;
		}
	}

	handleLoginAttempt();
</script>

<main>
	{#if assumeLoggedIn}
  		<Authenticated />
	{:else}
		<LandingZone />
	{/if}
</main>