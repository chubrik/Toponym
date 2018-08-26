export interface IResponse {
    status: Status;
    entries: IEntry[] | null;
    matchCount: number;
}

export interface IGroup extends IResponse {
    value: string;
    lastValue: string;
    category: EntryCategory;
    lastCategory: EntryCategory;
    isLoading: number;
    isIntensive: boolean;
}

export const enum Status {
    Success = 1,
    SyntaxError = 2,
    Failure = 3
}

export interface IEntry {
    title: string;
    type: EntryType;
    geo: number[];
    screen: number[][];
    _categoryClass: string;
    _isExpanded: boolean;
}

export const enum EntryCategory {
    Unknown = 0,
    Populated = 1,
    Water = 2,
    Locality = 4
}

export const allEntryCategories =
    EntryCategory.Populated | EntryCategory.Water | EntryCategory.Locality;

export const enum EntryType {
    Unknown = 0,

    // Populated
    Populated = 10,
    City,
    Dwelling,
    Hamlet,
    Town,
    Village,

    // Water
    Water = 20,
    Lake,
    Pond,
    River,
    Stream,

    // Locality
    Locality = 30
}

export const enum Language {
    Russian = 1,
    Belarusian = 2,
    English = 3
}
