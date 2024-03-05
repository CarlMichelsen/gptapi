import type { ConversationDto } from "../types/dto/conversation";
import type { ConversationOptionDateChunk } from "../types/dto/conversationOption";
import type { ServiceResponse } from "../types/dto/serviceResponse";
import { BaseServerClient } from "./baseClient";

export class ConversationClient extends BaseServerClient
{
    public async getConversationList(): Promise<ServiceResponse<ConversationOptionDateChunk[]>> {
        const endpoint = `${BaseServerClient.baseEndpoint}/api/v1/conversation`;
        return await this.request<ConversationOptionDateChunk[], void>(endpoint, "GET");
    }

    public async getConversation(conversationId: string): Promise<ServiceResponse<ConversationDto>> {
        const endpoint = `${BaseServerClient.baseEndpoint}/api/v1/conversation/${conversationId}`;
        return await this.request<ConversationDto, void>(endpoint, "GET");
    }

    public async deleteConversation(conversationId: string): Promise<ServiceResponse<boolean>> {
        const endpoint = `${BaseServerClient.baseEndpoint}/api/v1/conversation/${conversationId}`;
        return await this.request<boolean, void>(endpoint, "DELETE");
    }
}