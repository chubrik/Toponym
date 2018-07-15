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

    if (entry.type >= 100 && entry.type < 200)
        return langText('нп.', 'нп.', 'pop.');

    switch (entry.type) {

        case EntryType.WaterUnknown:
            return langText('вод.', 'вад.', 'wat.');

        case EntryType.River:
            return langText('р.', 'р.', 'riv.');

        case EntryType.Stream:
            return langText('руч.', 'руч.', 'str.');

        case EntryType.Lake:
            return langText('оз.', 'воз.', 'lake');

        case EntryType.Pond:
            return langText('пруд', 'саж.', 'pond');

        default:
            throw outOfRange('entry.type');
    }
}

export function entryTypeText(entry: IEntry): string {
    checkArgument(entry, 'entry');

    switch (entry.type) {

        case EntryType.PopulatedUnknown:
            return langText('Населённый пункт', 'Населены пункт', 'Populated locality');

        //case EntryType.Agrogorodok:
        //    return langText('Агрогородок', 'Аграгарадок', 'Agrotown');

        //case EntryType.Gorod:
        //    return langText('Город', 'Горад', 'City');

        //case EntryType.GorodskojPoselok:
        //    return langText('Городской посёлок', 'Гарадскі пасёлак', 'Urban settlement');

        //case EntryType.Derevnya:
        //    return langText('Деревня', 'Вёска', 'Hamlet');

        //case EntryType.KurortnyPoselok:
        //    return langText('Курортный посёлок', 'Курортны пасёлак', 'Resort settlement');

        //case EntryType.Poselok:
        //    return langText('Посёлок', 'Пасёлак', 'Settlement');

        //case EntryType.PoselokGorodskogoTipa:
        //    return langText(
        //        'Посёлок городского типа', 'Пасёлак гарадскога тыпу', 'Urban-type settlement');

        //case EntryType.RabochiPoselok:
        //    return langText('Рабочий посёлок', 'Працоўны пасёлак', 'Working settlement');

        //case EntryType.Selo:
        //    return langText('Село', 'Сяло', 'Village');

        //case EntryType.SelskiNaselennyPunkt:
        //    return langText(
        //        'Сельский населённый пункт', 'Сельскі населены пункт', 'Rural settlement');

        //case EntryType.Hutor:
        //    return langText('Хутор', 'Хутар', 'Bowery');

        case EntryType.WaterUnknown:
            return langText('Водоём', 'Вадаём', 'Water');

        case EntryType.River:
            return langText('Река', 'Рака', 'River');

        case EntryType.Stream:
            return langText('Ручей', 'Ручай', 'Stream');

        case EntryType.Lake:
            return langText('Озеро', 'Возера', 'Lake');

        case EntryType.Pond:
            return langText('Пруд', 'Сажалка', 'Pond');

        default:
            throw outOfRange('entry.type');
    }
}

export function linkLoadmap(entry: IEntry): string {
    checkArgument(entry, 'entry');

    return `${'http://'}m.loadmap.net/${langText('ru', 'ru', 'en')}` +
        `?qq=${entry.geo[0]}%20${entry.geo[1]}&z=13&s=100000&c=41&g=1`;
}

export function linkOsm(entry: IEntry): string {
    checkArgument(entry, 'entry');

    return `${'http://'}www.openstreetmap.org/` +
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
