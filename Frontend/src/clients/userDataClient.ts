import { baseUrl } from "../baseurl";
import type { SteamPlayer } from "../types/dto/steamPlayer";

export const getUserData = async (): Promise<SteamPlayer | null> => {
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

        return await response.json() as SteamPlayer;
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

export const navigateToLoginPage = () => {
    const loginPage = `${baseUrl()}/api/v1/oauth/SteamLogin`;
    window.location.replace(loginPage);
}