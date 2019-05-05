import { Language, IEntry, EntryType, IGroup } from './types';
import { language } from './app.module';
import { checkArgument, invalidArgument, outOfRange } from './errors';

/** Ни одно из: undefined, null, NaN, Infinity, -Infinity.
  * Любое значение даст true, включая: '', 0, false */
export function has(value: any): boolean {
    return value != null && !(typeof value === 'number' && !isFinite(value));
}

/** @param cases [один, два?, много] */
export function rusCase(rawNum: number, cases: string[], includeNumber = true): string {
    checkArgument(rawNum, 'number');
    checkArgument(cases, 'cases');

    if (cases.length < 2 || cases.length > 3 || cases.some(i => !i))
        throw invalidArgument('cases');

    const num = rawNum.toString();
    const result = includeNumber ? num + ' ' : '';

    if (num.slice(-2, -1) === '1')
        return result + (cases[2] || cases[1]);

    if (num.slice(-1) === '1')
        return result + cases[0];

    if (/[234]/.test(num.slice(-1)))
        return result + cases[1];

    return result + (cases[2] || cases[1]);
}

export function langText(russian: string, belarusian: string, english: string): string {
    checkArgument(russian, 'russian');
    checkArgument(belarusian, 'belarusian');
    checkArgument(english, 'english');

    switch (language) {

        case Language.Russian:
            return russian;

        case Language.Belarusian:
            return belarusian;

        case Language.English:
            return english;

        default:
            throw outOfRange('language');
    }
}

export function pointEntries(group: IGroup): IEntry[] | null {
    checkArgument(group, 'group');

    return group.entries
        ? group.entries.filter(i => i.type !== EntryType.River && i.type !== EntryType.Stream)
        : null;
}

export function polylineEntries(group: IGroup): IEntry[] | null {
    checkArgument(group, 'group');

    return group.entries
        ? group.entries.filter(i => i.type === EntryType.River || i.type === EntryType.Stream)
        : null;
}

export function polylinePoints(entry: IEntry): string {
    checkArgument(entry, 'entry');

    return entry.screen.map(s => s.join(',')).join(' ');
}

export function entryTypeAbbr(entry: IEntry): string {
    checkArgument(entry, 'entry');

    switch (entry.type) {

        // case EntryType.Populated:
        //     return langText('нп.', 'нп.', 'pop.');

        case EntryType.City:
            return langText('г.', 'г.', 'c.');

        case EntryType.Dwelling:
            return langText('х.', 'х.', 'dw.');

        case EntryType.Hamlet:
            return langText('д.', 'в.', 'h.');

        case EntryType.Town:
            return langText('г.', 'г.', 't.');

        case EntryType.Village:
            return langText('п.', 'п.', 'v.');

        // case EntryType.Water:
        //     return langText('вод.', 'вад.', 'wat.');

        case EntryType.Lake:
            return langText('оз.', 'воз.', 'lake');

        case EntryType.Pond:
            return langText('пруд', 'саж.', 'pond');

        case EntryType.River:
            return langText('р.', 'р.', 'riv.');

        case EntryType.Stream:
            return langText('руч.', 'руч.', 'str.');

        case EntryType.Locality:
            return langText('ур.', 'ур.', 'loc.');

        default:
            throw outOfRange('entry.type');
    }
}

export function entryTypeText(entry: IEntry): string {
    checkArgument(entry, 'entry');

    switch (entry.type) {

        // case EntryType.Populated:
        //     return langText('Населённый пункт', 'Населены пункт', 'Populated place');

        case EntryType.City:
            return langText('Город', 'Горад', 'City');

        case EntryType.Dwelling:
            return langText('Хутор', 'Хутар', 'Dwelling');

        case EntryType.Hamlet:
            return langText('Деревня', 'Вёска', 'Hamlet');

        case EntryType.Town:
            return langText('Город', 'Горад', 'Town');

        case EntryType.Village:
            return langText('Посёлок', 'Пасёлак', 'Village');

        // case EntryType.Water:
        //     return langText('Водный объект', 'Водны аб’ект', 'Water object');

        case EntryType.Lake:
            return langText('Озеро', 'Возера', 'Lake');

        case EntryType.Pond:
            return langText('Пруд', 'Сажалка', 'Pond');

        case EntryType.River:
            return langText('Река', 'Рака', 'River');

        case EntryType.Stream:
            return langText('Ручей', 'Ручай', 'Stream');

        case EntryType.Locality:
            return langText('Урочище', 'Урочышча', 'Locality');

        default:
            throw outOfRange('entry.type');
    }
}

export function linkOsm(entry: IEntry): string {
    checkArgument(entry, 'entry');

    return `${'https://'}www.openstreetmap.org/` +
        `?mlat=${entry.geo[0]}&mlon=${entry.geo[1]}&zoom=14`;
}

export function linkGoogle(entry: IEntry): string {
    checkArgument(entry, 'entry');

    return `${'https://'}www.google.${langText('ru', 'by', 'com')}/maps/` +
        `place//@${entry.geo[0]},${entry.geo[1]},5000m/data=!3m1!1e3!4m2!3m1!1s0x0:0x0?hl=ru`;
}

export function linkYandex(entry: IEntry): string {
    checkArgument(entry, 'entry');

    return `${`https://`}yandex.${langText('ru', 'by', 'com')}/maps` +
        `?ll=${entry.geo[1]},${entry.geo[0]}` +
        `&pt=${entry.geo[1]},${entry.geo[0]}&z=14&l=sat%2Cskl`;
}

export function linkLoadmap(entry: IEntry): string {
    checkArgument(entry, 'entry');

    return `${'http://'}loadmap.net/${langText('ru', 'ru', 'en')}` +
        `?qq=${entry.geo[0]}%20${entry.geo[1]}&z=13&s=100000&c=41&g=1`;
}
