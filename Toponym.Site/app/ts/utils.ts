import { Language, IItem, ItemType, IGroup } from './types';
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

export function pointItems(group: IGroup): IItem[] | null {
    checkArgument(group, 'group');

    return group.items
        ? group.items.filter(i => i.type !== ItemType.River && i.type !== ItemType.Stream)
        : null;
}

export function polylineItems(group: IGroup): IItem[] | null {
    checkArgument(group, 'group');

    return group.items
        ? group.items.filter(i => i.type === ItemType.River || i.type === ItemType.Stream)
        : null;
}

export function polylinePoints(item: IItem): string {
    checkArgument(item, 'item');

    return item.screen.map(s => s.join(',')).join(' ');
}

export function itemTypeAbbr(item: IItem): string {
    checkArgument(item, 'item');

    if (item.type >= 100 && item.type < 200)
        return langText('нп.', 'нп.', 'pop.');

    switch (item.type) {

        case ItemType.WaterUnknown:
            return langText('вод.', 'вад.', 'wat.');

        case ItemType.River:
            return langText('р.', 'р.', 'riv.');

        case ItemType.Stream:
            return langText('руч.', 'руч.', 'str.');

        case ItemType.Lake:
            return langText('оз.', 'воз.', 'lake');

        case ItemType.Pond:
            return langText('пруд', 'саж.', 'pond');

        default:
            throw outOfRange('item.type');
    }
}

export function itemTypeText(item: IItem): string {
    checkArgument(item, 'item');

    switch (item.type) {

        case ItemType.PopulatedUnknown:
            return langText('Населённый пункт', 'Населены пункт', 'Populated locality');

        //case ItemType.Agrogorodok:
        //    return langText('Агрогородок', 'Аграгарадок', 'Agrotown');

        //case ItemType.Gorod:
        //    return langText('Город', 'Горад', 'City');

        //case ItemType.GorodskojPoselok:
        //    return langText('Городской посёлок', 'Гарадскі пасёлак', 'Urban settlement');

        //case ItemType.Derevnya:
        //    return langText('Деревня', 'Вёска', 'Hamlet');

        //case ItemType.KurortnyPoselok:
        //    return langText('Курортный посёлок', 'Курортны пасёлак', 'Resort settlement');

        //case ItemType.Poselok:
        //    return langText('Посёлок', 'Пасёлак', 'Settlement');

        //case ItemType.PoselokGorodskogoTipa:
        //    return langText(
        //        'Посёлок городского типа', 'Пасёлак гарадскога тыпу', 'Urban-type settlement');

        //case ItemType.RabochiPoselok:
        //    return langText('Рабочий посёлок', 'Працоўны пасёлак', 'Working settlement');

        //case ItemType.Selo:
        //    return langText('Село', 'Сяло', 'Village');

        //case ItemType.SelskiNaselennyPunkt:
        //    return langText(
        //        'Сельский населённый пункт', 'Сельскі населены пункт', 'Rural settlement');

        //case ItemType.Hutor:
        //    return langText('Хутор', 'Хутар', 'Bowery');

        case ItemType.WaterUnknown:
            return langText('Водоём', 'Вадаём', 'Water');

        case ItemType.River:
            return langText('Река', 'Рака', 'River');

        case ItemType.Stream:
            return langText('Ручей', 'Ручай', 'Stream');

        case ItemType.Lake:
            return langText('Озеро', 'Возера', 'Lake');

        case ItemType.Pond:
            return langText('Пруд', 'Сажалка', 'Pond');

        default:
            throw outOfRange('item.type');
    }
}

export function linkLoadmap(item: IItem): string {
    checkArgument(item, 'item');

    return `${'http://'}m.loadmap.net/${langText('ru', 'ru', 'en')}` +
        `?qq=${item.gps[0]}%20${item.gps[1]}&z=13&s=100000&c=41&g=1`;
}

export function linkOsm(item: IItem): string {
    checkArgument(item, 'item');

    return `${'http://'}www.openstreetmap.org/?mlat=${item.gps[0]}&mlon=${item.gps[1]}&zoom=14`;
}

export function linkGoogle(item: IItem): string {
    checkArgument(item, 'item');

    return `${'https://'}www.google.${langText('ru', 'by', 'com')}/maps/` +
        `place//@${item.gps[0]},${item.gps[1]},5000m/data=!3m1!1e3!4m2!3m1!1s0x0:0x0?hl=ru`;
}

export function linkYandex(item: IItem): string {
    checkArgument(item, 'item');

    return `${`https://`}yandex.${langText('ru', 'by', 'com')}/maps` +
        `?ll=${item.gps[1]},${item.gps[0]}&pt=${item.gps[1]},${item.gps[0]}&z=14&l=sat%2Cskl`;
}
