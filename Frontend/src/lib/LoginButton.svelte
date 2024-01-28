<script lang="ts">
    import { navigateToLoginPage, type LoginType } from "../clients/userDataClient";
    export let loginType: LoginType;

    type ButtonInformation = { text: string };

    const buttonInformationMap: Record<LoginType, ButtonInformation> = {
        "Development": { text: "Development Login" },
        "Guest": { text: "Guest with Steam" },
        "Discord": { text: "Login with Discord" },
        "Github": { text: "Login with Github" },
    }

    const buttonInformation: ButtonInformation = buttonInformationMap[loginType];
</script>

<button
    class="p-2 bg-zinc-400 text-black hover:bg-zinc-600 hover:text-white active:underline rounded-md shadow-2xl block mx-auto h-12 w-full"
    on:click={() => navigateToLoginPage(loginType)}>
    <p>{buttonInformation.text}</p>
</button>