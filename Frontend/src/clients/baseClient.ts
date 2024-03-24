import { baseUrl, loginUrl } from "../baseurl";
import type { ServiceResponse } from "../types/dto/conversation/serviceResponse";

export abstract class BaseServerClient
{
    protected static baseEndpoint = baseUrl();
    protected static loginEndpoint = loginUrl();

    protected async request<T, P>(endpoint: string, method: string, payload?: P): Promise<ServiceResponse<T>>
    {
        try {
            const reqInit: RequestInit = {
                method,
                headers: {
                  'Content-Type': 'application/json',
                  'Accept': 'application/json',
                },
                credentials: "include",
            }

            if (payload) {
                reqInit.body = JSON.stringify(payload);
            }

            const response = await fetch(endpoint, reqInit);
            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }

            return await response.json() as ServiceResponse<T>;
        } catch (error) {
            console.error(error);
            return {
                ok: false,
                data: null,
                errors: ["An error occured"]
            };
        }
    }
}