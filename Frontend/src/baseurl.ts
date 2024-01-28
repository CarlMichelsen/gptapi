export const baseUrl = (): string => {
    const isDev = import.meta.env.MODE == "development";
    return isDev ? "http://localhost:5142" : "";
}

export const loginUrl = (): string => {
    const isDev = import.meta.env.MODE == "development";
    return isDev ? "http://localhost:5197" : "https://login.survivethething.com";
}