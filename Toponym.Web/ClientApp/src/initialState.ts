import { Language } from './types';

export interface InitialState {
  language: Language;
  fbAppId: string;
  defaultHost: string;
}

const defaults: InitialState = {
  language: Language.Russian,
  fbAppId: '',
  defaultHost: 'toponim.by',
};

export function readInitialState(): InitialState {
  const el = document.getElementById('__toponym_state');
  if (!el?.textContent) return defaults;
  try {
    return { ...defaults, ...JSON.parse(el.textContent) };
  } catch {
    return defaults;
  }
}
