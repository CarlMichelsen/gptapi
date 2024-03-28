import type { AvailableModel } from "../dto/availableModel/availableModel";

export type Preferences = {
    lastSelectedModel: AvailableModel;
};

export type LocalStore = {
    preferences: Preferences | null;
};