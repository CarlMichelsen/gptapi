import { baseUrl } from "../baseurl";
import type { ConversationMetadata, ConversationType } from "../types/dto/conversation"
import type { ServiceResponse } from "../types/dto/serviceResponse";

export const getConversationList = async () : Promise<ServiceResponse<ConversationMetadata[]>> => {
    const endpoint = `${baseUrl()}/api/v1/conversation`;
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

        return await response.json() as ServiceResponse<ConversationMetadata[]>;
    } catch (error) {
        console.error(error);
        return {
            ok: false,
            data: null,
            errors: ["An error occured"]
        };
    }
}

export const getConversation = async (conversationId: string) : Promise<ServiceResponse<ConversationType>> => {
    const endpoint = `${baseUrl()}/api/v1/conversation/${conversationId}`;
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

        return await response.json() as ServiceResponse<ConversationType>;
    } catch (error) {
        console.error(error);
        return {
            ok: false,
            data: null,
            errors: ["An error occured"]
        };
    }
}

export const deleteConversation = async (conversationId: string) : Promise<ServiceResponse<boolean>> => {
    const endpoint = `${baseUrl()}/api/v1/conversation/${conversationId}`;
    try {
        const response = await fetch(endpoint, {
            method: 'DELETE',
            headers: {
              'Content-Type': 'application/json',
              'Accept': 'application/json',
            },
            credentials: "include",
        });

        if (!response.ok) {
            throw new Error(`Error: ${response.status}`);
        }

        return await response.json() as ServiceResponse<boolean>;
    } catch (error) {
        console.error(error);
        return {
            ok: false,
            data: null,
            errors: ["An error occured"]
        };
    }
}