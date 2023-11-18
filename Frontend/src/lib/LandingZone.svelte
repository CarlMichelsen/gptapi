<script lang="ts">
    import { navigateToLoginPage } from "../clients/userDataClient";


</script>

<div>
    <p>LandingZone</p>

    <button on:click={() => navigateToLoginPage()}>Login</button>
</div>