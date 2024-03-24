import type { ConversationDto } from "../types/dto/conversation/conversation";
import type { ConversationOptionDto } from "../types/dto/conversation/conversationOption";
import type { ServiceResponse } from "../types/dto/conversation/serviceResponse";
import { BaseServerClient } from "./baseClient";

export class ConversationClient extends BaseServerClient
{
    public async getConversationList(): Promise<ServiceResponse<ConversationOptionDto[]>> {
        const endpoint = `${BaseServerClient.baseEndpoint}/api/v1/conversation`;
        return await this.request<ConversationOptionDto[], void>(endpoint, "GET");
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