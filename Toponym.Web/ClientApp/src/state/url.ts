import { allEntryCategories, EntryCategory } from '../types';
import type { Group } from '../types';
import { createGroup } from './store';

export interface ParsedQuery {
  value: string;
  category: EntryCategory;
}

export function parseUrl(search: string): ParsedQuery[] {
  const params = new URLSearchParams(search);
  const result: ParsedQuery[] = [];
  let found = false;

  for (let i = 5; i > 0; i--) {
    const value = params.get('q' + i);
    if (!value && !found) continue;
    found = true;
    const rawCategory = Number(params.get('t' + i));
    const category =
      !rawCategory || rawCategory <= 0 || rawCategory > allEntryCategories
        ? allEntryCategories
        : (rawCategory as EntryCategory);
    result.unshift({ value: value ?? '', category });
  }

  return result;
}

export function groupsFromQueries(queries: ParsedQuery[]): Group[] {
  if (!queries.length) return [createGroup()];
  return queries.map((q) => createGroup(q.value, q.category));
}

export function buildSearch(groups: Group[]): string {
  const params = new URLSearchParams();
  for (let i = 0; i < groups.length; i++) {
    const g = groups[i];
    if (g.value) params.set('q' + (i + 1), g.value);
    if (g.category && g.category !== allEntryCategories) {
      params.set('t' + (i + 1), String(g.category));
    }
  }
  const s = params.toString();
  return s ? '?' + s : '';
}
