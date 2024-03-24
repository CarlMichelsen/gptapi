type ServiceResponseOk<T> = {
    ok: true;
    data: T;
    errors: null;
};

type ServiceResponseNotOk = {
    ok: false;
    data: null;
    errors: string[];
};

export type ServiceResponse<T> = ServiceResponseOk<T> | ServiceResponseNotOk;