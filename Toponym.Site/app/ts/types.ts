export interface IResponse {
    status: Status;
    items: IItem[] | null;
    matchCount: number;
}

export interface IGroup extends IResponse {
    value: string;
    lastValue: string;
    type: GroupType;
    lastType: GroupType;
    isLoading: number;
    isIntensive: boolean;
}

export const enum Status {
    Success = 1,
    SyntaxError = 2,
    Failure = 3
}

export interface IItem {
    title: string;
    type: ItemType;
    gps: number[];
    screen: number[][];
    _typeClass: string;
    _isExpanded: boolean;
}

export const enum GroupType {
    All = 0,
    Populated = 1,
    Water = 2
}

export const enum ItemType {
    Unknown = 0,

    // Населённые пункты
    PopulatedUnknown = 100,
    Agrogorodok,
    Gorod,
    GorodskojPoselok,
    Derevnya,
    KurortnyPoselok,
    Poselok,
    PoselokGorodskogoTipa,
    RabochiPoselok,
    Selo,
    SelskiNaselennyPunkt,
    Hutor,

    // Водные объекты
    WaterUnknown = 200,
    River,
    Stream,
    Lake,
    Pond

    // Объекты местности
}

export const enum Language {
    Russian = 1,
    Belarusian = 2,
    English = 3
}
