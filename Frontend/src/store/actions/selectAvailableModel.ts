import type { AvailableModel } from "../../types/dto/availableModel/availableModel";
import type { ApplicationStore } from "../../types/store/applicationStore";
import type { StoreFunction } from "../storeFunction";

export const selectAvailableModel: StoreFunction<[AvailableModel]> = (state: ApplicationStore, model: AvailableModel): ApplicationStore => {
    if (state.state === "logged-out") return state;
    if (state.languageModel === null) return state;

    return {
        ...state,
        languageModel: {
            availableModels: state.languageModel.availableModels,
            selectedModel: model,
        }
    }
}