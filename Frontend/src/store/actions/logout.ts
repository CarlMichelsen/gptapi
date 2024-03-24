import type { ApplicationStore, LoggedOutApplicationStore } from "../../types/store/applicationStore";
import type { StoreFunction } from "../storeFunction";

export const logout: StoreFunction<[]> = (_: ApplicationStore): ApplicationStore => {
    return {
        ready: true,
        state: "logged-out",
    } as LoggedOutApplicationStore;
}