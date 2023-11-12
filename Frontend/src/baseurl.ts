export const baseUrl = (): string => {
    const isDev = import.meta.env.MODE == "development";
    return isDev ? "http://localhost:5142" : "";
}