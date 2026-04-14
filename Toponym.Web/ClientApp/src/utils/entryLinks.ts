import { EntryType } from '../types';
import type { Entry } from '../types';
import { langText } from '../i18n/lang';

export function isPolyline(entry: Entry): boolean {
  return entry.type === EntryType.River || entry.type === EntryType.Stream;
}

export function polylinePoints(entry: Entry): string {
  return entry.screen.map((s) => s.join(',')).join(' ');
}

export function linkOsm(entry: Entry): string {
  return `https://www.openstreetmap.org/?mlat=${entry.geo[0]}&mlon=${entry.geo[1]}&zoom=14`;
}

export function linkGoogle(entry: Entry): string {
  return (
    `https://www.google.${langText('ru', 'by', 'com')}/maps/` +
    `place//@${entry.geo[0]},${entry.geo[1]},5000m/data=!3m1!1e3!4m2!3m1!1s0x0:0x0?hl=ru`
  );
}

export function linkYandex(entry: Entry): string {
  return (
    `https://yandex.${langText('ru', 'by', 'com')}/maps` +
    `?ll=${entry.geo[1]},${entry.geo[0]}` +
    `&pt=${entry.geo[1]},${entry.geo[0]}&z=14&l=sat%2Cskl`
  );
}

export function linkLoadmap(entry: Entry): string {
  return (
    `http://m.loadmap.net/${langText('ru', 'ru', 'en')}` +
    `?qq=${entry.geo[0]}%20${entry.geo[1]}&z=13&s=100000&c=41&g=1`
  );
}
