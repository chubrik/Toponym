import { EntryType } from '../types';
import type { Entry } from '../types';
import { langText } from './lang';

export function entryTypeAbbr(entry: Entry): string {
  switch (entry.type) {
    case EntryType.City: return langText('г.', 'г.', 'c.');
    case EntryType.Dwelling: return langText('х.', 'х.', 'dw.');
    case EntryType.Hamlet: return langText('д.', 'в.', 'h.');
    case EntryType.Town: return langText('г.', 'г.', 't.');
    case EntryType.Village: return langText('п.', 'п.', 'v.');
    case EntryType.Lake: return langText('оз.', 'воз.', 'lake');
    case EntryType.Pond: return langText('пруд', 'саж.', 'pond');
    case EntryType.River: return langText('р.', 'р.', 'riv.');
    case EntryType.Stream: return langText('руч.', 'руч.', 'str.');
    case EntryType.Locality: return langText('ур.', 'ур.', 'loc.');
    default: return '';
  }
}

export function entryTypeText(entry: Entry): string {
  switch (entry.type) {
    case EntryType.City: return langText('Город', 'Горад', 'City');
    case EntryType.Dwelling: return langText('Хутор', 'Хутар', 'Dwelling');
    case EntryType.Hamlet: return langText('Деревня', 'Вёска', 'Hamlet');
    case EntryType.Town: return langText('Город', 'Горад', 'Town');
    case EntryType.Village: return langText('Посёлок', 'Пасёлак', 'Village');
    case EntryType.Lake: return langText('Озеро', 'Возера', 'Lake');
    case EntryType.Pond: return langText('Пруд', 'Сажалка', 'Pond');
    case EntryType.River: return langText('Река', 'Рака', 'River');
    case EntryType.Stream: return langText('Ручей', 'Ручай', 'Stream');
    case EntryType.Locality: return langText('Урочище', 'Урочышча', 'Locality');
    default: return '';
  }
}
