<script lang="ts">
    import { applicationStore } from "./store/applicationStore";
  	import Authenticated from "./lib/Authenticated.svelte";
    import LandingZone from "./lib/LandingZone.svelte";
    import { getUserData } from "./clients/userDataClient";

	const attemptLogin = async () => {
		const oauthUserResponse = await getUserData();

		if (oauthUserResponse.ok) {
			applicationStore.login(oauthUserResponse.data);
		} else {
			// This simple sets the applicationState to "logged-out" and marks the applications as "ready"
			applicationStore.logout();
		}
	}

	attemptLogin();
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