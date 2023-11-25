export const setQueryParam = (key: string, value: string|null|undefined) => {
    // Create a URLSearchParams object based on current search string
    const searchParams = new URLSearchParams(window.location.search);

    // Set the new or updated query parameter
    if (value) {
        searchParams.set(key, value);
    } else {
        searchParams.delete(key);
    }

    // Use pushState to update the URL without reloading the page
    const newUrl = `${window.location.pathname}?${searchParams}`;
    window.history.pushState({}, '', newUrl);
}

export const getQueryParams = () => {
    const params = new URLSearchParams(window.location.search);
    let queryObj: { [key: string]: string } = {};
    params.forEach((value, key) => {
        queryObj[key] = value;
    });
    return queryObj;
}