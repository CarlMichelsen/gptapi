import type { OAuthUser } from "../../types/dto/oAuthUser";
import type { ApplicationStore } from "../../types/store/applicationStore";
import type { StoreFunction } from "../storeFunction";

export const login: StoreFunction<[OAuthUser]> = (
    state: ApplicationStore,
    oAuthUser: OAuthUser): ApplicationStore => {
    if (state.state === "logged-in") return state;
    
    return {
        ...state,
        ready: true,
        state: "logged-in",
        user: oAuthUser,
        selectedConversation: null,
        conversationChunks: null,
        languageModel: null,
    };
}