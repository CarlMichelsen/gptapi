export const forceDigitCount = (num: number, digitCount: number = 2): string => {
    const strNum = num.toString();
    if (strNum.length >= digitCount) {
        return strNum;
    }

    return `${"0".repeat(digitCount-strNum.length)}${strNum}`;
}

export const displayDate = (date: Date): string => {
    const safeDate = new Date(date);
    return `${forceDigitCount(safeDate.getDay())}.${forceDigitCount(safeDate.getMonth())}.${safeDate.getFullYear()}`;
}