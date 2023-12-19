<script lang="ts">
    import type { LoginType } from "../clients/userDataClient";
    import LoginButton from "./LoginButton.svelte";

    const loginTypes: LoginType[] = [ "Discord", "Github" ];

    if (process.env.NODE_ENV === 'development') {
        loginTypes.push("Development");
    }
</script>

<div class="lg:container mx-auto lg:p-2 lg:mt-5">
    <article class="max-w-4xl mx-auto p-6">
        <header class="text-white p-4 rounded-t-lg">
            <h1 class="text-2xl font-bold">CarlGPT: A Totally "New" Chat Experience</h1>
            <p class="mt-1 text-lg">A groundbreaking, "original" platform awaits you.</p>
        </header>
        <section class="p-4">
            <p class="text-sm text-zinc-400">Welcome to <strong class="text-white">CarlGPT</strong>, the chatbot revolutionizing the way we think about AI! Our "new" system promises a user experience so strikingly innovative, it will feel like déjà vu.</p>
        </section>
        <section class="p-4 border-t border-gray-200">
            <h2 id="not-openai" class="font-semibold text-xl">Definitely Not OpenAI's Work</h2>
            <p class="mt-2">Join the ranks of amazed users who find CarlGPT's abilities eerily reminiscent of something they know and love. It's totally "not" the OpenAI API, we promise.</p>
        </section>
        <section class="p-4 border-t border-gray-200">
            <h2 id="memory-capabilities" class="font-semibold text-xl">Memory That "Never Forgets"</h2>
            <p class="mt-2">Our platform "remembers" your chats with stunning accuracy. With CarlGPT, rest assured your conversations are as new as they are familiar.</p>
        </section>
        
        <section class="p-4 border-t border-gray-200">
            <h2 id="get-started" class="font-semibold text-xl text-center">Start using CarlGPT</h2>

            <br />

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4 w-72 md:w-96 mx-auto">
                {#each loginTypes as buttonInfo}
                    <LoginButton loginType={buttonInfo} />
                {/each}
            </div>

            <br />
        </section>

        <section class="p-4 border-t border-gray-200">
            <h2 id="eco-friendly" class="font-semibold text-xl">Eco-Friendly (We Swear)</h2>
            <p class="mt-2">CarlGPT is "green" thanks to our sustainable infrastructure that's completely unique (wink, wink) and definitely not inherited.</p>
        </section>
        <footer class="text-zinc-400 p-4 rounded-b-lg">
            <p>Get started with CarlGPT today, and marvel at the chatbot experience you've never "seen" before!</p>
        </footer>
    </article>

    <br />
</div>