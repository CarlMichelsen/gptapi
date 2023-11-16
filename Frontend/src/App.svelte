<script lang="ts">
    import { getUserData } from "./clients/userDataClient";
  	import Authenticated from "./lib/Authenticated.svelte";
    import LandingZone from "./lib/LandingZone.svelte";
    import type { SteamPlayer } from "./types/dto/steamPlayer";
	
	let userData: SteamPlayer | null = null;

	const removeQueryParamsWithoutReloading = () => {
    	if (window.history.replaceState) {
			const url = new URL(window.location.href);
			url.search = ""; // Clear all query parameters

			// Replace the current state without pushing a new state and without reloading
			window.history.replaceState({}, document.title, url.pathname);
		}
	}

	const handleLoginAttempt = async () => {
		const url = new URL(window.location.toString());
		const login = url.searchParams.get("login") === "true";
		if (login) removeQueryParamsWithoutReloading();
		
		const data = await getUserData();
		console.log("handleLoginAttempt", data);
		if (data != null) {
			userData = data;
			removeQueryParamsWithoutReloading();
		}
	}

	handleLoginAttempt();
</script>

<main>
	{#if userData != null}
  		<Authenticated userData={userData} />
	{:else}
		<LandingZone />
	{/if}
</main>