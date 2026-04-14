import { Status } from '../types';
import type { EntryCategory, Language, SearchResponse } from '../types';

function readMeta(name: string): string {
  const el = document.querySelector<HTMLMetaElement>(`meta[name="${name}"]`);
  return el?.content ?? '';
}

function formEncode(data: Record<string, string | number>): string {
  return Object.entries(data)
    .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(String(value))}`)
    .join('&');
}

export async function fetchEntries(
  query: string,
  category: EntryCategory,
  language: Language,
): Promise<SearchResponse> {
  const tokenField = readMeta('csrf-field') || '__RequestVerificationToken';
  const tokenValue = readMeta('csrf-token');

  const body = formEncode({
    query,
    category,
    language,
    [tokenField]: tokenValue,
  });

  const response = await fetch('/xhr/entries', {
    method: 'POST',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=utf-8' },
    body,
    credentials: 'same-origin',
  });

  if (!response.ok) {
    return { status: Status.Failure, entries: null, matchCount: 0 };
  }

  const data = (await response.json()) as Partial<SearchResponse>;
  return {
    status: data.status ?? Status.Failure,
    entries: data.entries ?? null,
    matchCount: data.matchCount ?? 0,
  };
}
