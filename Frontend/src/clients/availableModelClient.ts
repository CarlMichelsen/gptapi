import type { AvailableModelResponse } from "../types/dto/availableModel";
import { BaseServerClient } from "./baseClient";

export class AvailableModelClient extends BaseServerClient
{
    public async getAvailableModel() {
        const endpoint = `${BaseServerClient.baseEndpoint}/api/v1/availableModel`;
        return await this.request<AvailableModelResponse, void>(endpoint, "GET");
    }
}