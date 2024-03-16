import type { AvailableModel } from "./availableModel";

export type AvailableModelResponse = {
    availableModels: { [key: string]: AvailableModel[] };
};