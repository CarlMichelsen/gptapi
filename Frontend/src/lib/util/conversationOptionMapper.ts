import type { ConversationOptionDateChunk, ConversationOptionDto } from "../../types/dto/conversationOption";

export const conversationOptionMapper = (options: ConversationOptionDto[]): ConversationOptionDateChunk[] => {
    const groupedOptions = groupBy(options, (o) => {
        const totalDays = (new Date().getTime() - new Date(o.lastAppendedUtc).getTime()) / (1000 * 3600 * 24);
        if (totalDays < 1) {
            return "Today";
        } else if (totalDays < 2) {
            return "Yesterday";
        } else if (totalDays < 7) {
            return "This week";
        } else if (totalDays < 30) {
            return "This month";
        } else if (totalDays < 365) {
            return "This year";
        }
        return "Older than a year";
    });

    const conversationDateChunks = groupedOptions.map(([label, groupOptions]): ConversationOptionDateChunk => ({
        dateText: label,
        options: groupOptions.toSorted((a, b) => new Date(b.lastAppendedUtc).getTime() - new Date(a.lastAppendedUtc).getTime()),
    }));

    return conversationDateChunks.sort(
        (a, b) => new Date(b.options[0]!.lastAppendedUtc).getTime() - new Date(a.options[0]!.lastAppendedUtc).getTime()
    );
}

const groupBy = <T, K>(list: T[], getKey: (item: T) => K): [K, T[]][] => {
    const map = new Map<K, T[]>();
    for (const item of list) {
        const key = getKey(item);
        const collection = map.get(key);
        if (!collection) {
            map.set(key, [item]);
        } else {
            collection.push(item);
        }
    }
    return Array.from(map.entries());
}