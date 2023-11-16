import { baseUrl } from "../baseurl";
import type { SteamPlayer } from "../types/dto/steamPlayer";

export const getUserData = async (): Promise<SteamPlayer | null> => {
    const endpoint = `${baseUrl()}/api/v1/oauth/UserData`;
    try {
        const response = await fetch(endpoint, {
            method: 'GET',
            headers: {
              'Content-Type': 'application/json',
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