import { baseUrl } from "../baseurl";
import type { ConversationMetadata, ConversationType } from "../types/dto/conversation"

export const getConversationOptions = async () : Promise<ConversationMetadata[]|null> => {
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

        return await response.json() as ConversationMetadata[];
    } catch (error) {
        console.error(error);
        return null;
    }
}

export const getConversation = async (conversationId: string) : Promise<ConversationType|null> => {
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

        return await response.json() as ConversationType;
    } catch (error) {
        console.error(error);
        return null;
    }
}

export const deleteConversation = async (conversationId: string) : Promise<boolean> => {
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

        return true;
    } catch (error) {
        console.error(error);
        return false;
    }
}