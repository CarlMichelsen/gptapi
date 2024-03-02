import type { ConversationMetadata, ConversationType } from "../types/dto/conversation";
import type { ServiceResponse } from "../types/dto/serviceResponse";
import { BaseServerClient } from "./baseClient";

export class ConversationClient extends BaseServerClient
{
    public async getConversationList(): Promise<ServiceResponse<ConversationMetadata[]>> {
        const endpoint = `${BaseServerClient.baseEndpoint}/api/v1/conversation`;
        return await this.request<ConversationMetadata[], void>(endpoint, "GET");
    }

    public async getConversation(conversationId: string): Promise<ServiceResponse<ConversationType>> {
        const endpoint = `${BaseServerClient.baseEndpoint}/api/v1/conversation/${conversationId}`;
        return await this.request<ConversationType, void>(endpoint, "GET");
    }

    public async deleteConversation(conversationId: string): Promise<ServiceResponse<boolean>> {
        const endpoint = `${BaseServerClient.baseEndpoint}/api/v1/conversation/${conversationId}`;
        return await this.request<boolean, void>(endpoint, "DELETE");
    }
}