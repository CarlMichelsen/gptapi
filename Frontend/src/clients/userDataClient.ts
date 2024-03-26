import { baseUrl, loginUrl } from "../baseurl";
import type { ServiceResponse } from "../types/dto/conversation/serviceResponse";
import type { OAuthUser } from "../types/dto/oAuthUser";

export const getUserData = async (): Promise<ServiceResponse<OAuthUser>> => {
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

        return await response.json() as ServiceResponse<OAuthUser>;
    } catch (error) {
        return {
            ok: false,
            data: null,
            errors: ["Error"]
        };
    }
}

export const deleteCookie = async () => {
    const endpoint = `${loginUrl()}/api/v1/Login/Logout`;
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

export type LoginType = "Development" | "Guest" | "Github" | "Discord";

export const navigateToLoginPage = (loginType: LoginType) => {
    const endpoints: Record<LoginType, string> = {
        "Development": "/api/v1/Login/Development",
        "Guest": "/api/v1/Login/Guest",
        "Github": "/api/v1/Login/Github",
        "Discord": "/api/v1/Login/Discord"
    };

    const loginPage = `${loginUrl()}${endpoints[loginType]}?redirect=${encodeURIComponent(document.location.origin)}`;
    window.location.replace(loginPage);
}