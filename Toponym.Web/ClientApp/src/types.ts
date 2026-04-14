export enum Language {
  Russian = 1,
  Belarusian = 2,
  English = 3,
}

export enum Status {
  Success = 1,
  SyntaxError = 2,
  Failure = 3,
}

export enum EntryCategory {
  Unknown = 0,
  Populated = 1,
  Water = 2,
  Locality = 4,
}

export const allEntryCategories =
  EntryCategory.Populated | EntryCategory.Water | EntryCategory.Locality;

export enum EntryType {
  Unknown = 0,

  Populated = 10,
  City = 11,
  Dwelling = 12,
  Hamlet = 13,
  Town = 14,
  Village = 15,

  Water = 20,
  Lake = 21,
  Pond = 22,
  River = 23,
  Stream = 24,

  Locality = 30,
}

export interface Entry {
  title: string;
  type: EntryType;
  geo: number[];
  screen: number[][];
}

export interface SearchResponse {
  status: Status;
  entries: Entry[] | null;
  matchCount: number;
}

export interface Group {
  id: number;
  value: string;
  category: EntryCategory;
  lastValue: string;
  lastCategory: EntryCategory;
  status: Status | null;
  entries: Entry[] | null;
  matchCount: number;
  isLoading: boolean;
}

export function entryCategory(type: EntryType): EntryCategory {
  if (type >= EntryType.Populated && type < EntryType.Water) return EntryCategory.Populated;
  if (type >= EntryType.Water && type < EntryType.Locality) return EntryCategory.Water;
  if (type >= EntryType.Locality) return EntryCategory.Locality;
  return EntryCategory.Unknown;
}
