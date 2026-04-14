import { Language } from '../types';

let currentLanguage: Language = Language.Russian;

export function setLanguage(lang: Language): void {
  currentLanguage = lang;
}

export function getLanguage(): Language {
  return currentLanguage;
}

export function langText(russian: string, belarusian: string, english: string): string {
  switch (currentLanguage) {
    case Language.Russian:
      return russian;
    case Language.Belarusian:
      return belarusian;
    case Language.English:
      return english;
  }
}

export function rusCase(rawNum: number, cases: [string, string, string?], includeNumber = true): string {
  const num = rawNum.toString();
  const prefix = includeNumber ? num + ' ' : '';

  if (num.slice(-2, -1) === '1') return prefix + (cases[2] ?? cases[1]);
  if (num.slice(-1) === '1') return prefix + cases[0];
  if (/[234]/.test(num.slice(-1))) return prefix + cases[1];
  return prefix + (cases[2] ?? cases[1]);
}
