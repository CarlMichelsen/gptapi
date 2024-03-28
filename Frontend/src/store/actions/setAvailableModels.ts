import type { AvailableModel } from "../../types/dto/availableModel/availableModel";
import type { ApplicationStore, LanguageModel } from "../../types/store/applicationStore";
import type { StoreFunction } from "../storeFunction";

export const setAvailableModels: StoreFunction<[{ [provider: string]: AvailableModel[] }, AvailableModel]> = (
    state: ApplicationStore,
    availableModels: { [provider: string]: AvailableModel[] },
    initialModel: AvailableModel): ApplicationStore => {
    if (state.state !== "logged-in") return state;

    const lModel: LanguageModel = {
        availableModels: availableModels,
        selectedModel: initialModel,
    };

    return {
        ...state,
        languageModel: lModel
    }
}