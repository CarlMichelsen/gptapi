export type Credentials = {
    username: string;
    password: string;
};

export type LocalStore = {
    credentials: Credentials | undefined;
};