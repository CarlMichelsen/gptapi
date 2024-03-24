import type { ApplicationStore } from "../types/store/applicationStore";

type OmitFirstApplicationStoreTuple<T extends any[]> = T extends [ApplicationStore, ...infer Rest] ? Rest : [];

export type OmitApplicationStoreInitialParameter<T extends (...args: [ApplicationStore, ...any[]]) => any> = OmitFirstApplicationStoreTuple<Parameters<T>>;

export type StoreFunction<T extends any[] = []> = (store: ApplicationStore, ...args: [...T]) => ApplicationStore;