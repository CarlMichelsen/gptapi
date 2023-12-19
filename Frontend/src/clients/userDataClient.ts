import { baseUrl } from "../baseurl";
import type { OAuthUser } from "../types/dto/oAuthUser";

export const getUserData = async (): Promise<OAuthUser | null> => {
    const endpoint = `${baseUrl()}/api/v1/session/UserData`;
    try {
        const response = await fetch(endpoint, {
            method: 'POST',
            headers: {
              'Accept': 'application/json',
            },
            credentials: "include",
        });
        
        if (!response.ok) {
            throw new Error(`Error: ${response.status}`);
        }

        return await response.json() as OAuthUser;
    } catch (error) {
        return null;
    }
}

export const deleteCookie = async () => {
    const endpoint = `${baseUrl()}/api/v1/session/Logout`;
    try {
        const response = await fetch(endpoint, {
            method: 'DELETE',
            credentials: "include",
        });
        
        if (!response.ok) {
            throw new Error(`Error: ${response.status}`);
        }
    } catch (error) {
        console.error("deleteCookie", error);
    }
}

export type LoginType = "Development" | "Steam" | "Github" | "Discord";

export const navigateToLoginPage = (loginType: LoginType) => {
    const endpoints: Record<LoginType, string> = {
        "Development": "/api/v1/oauth/DevelopmentLogin",
        "Steam": "/api/v1/oauth/SteamLogin",
        "Github": "/api/v1/oauth/GithubLogin",
        "Discord": "/api/v1/oauth/DiscordLogin"
    };

    const loginPage = `${baseUrl()}${endpoints[loginType]}`;
    window.location.replace(loginPage);
}