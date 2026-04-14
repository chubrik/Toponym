import { useEffect } from 'react';
import { fetchEntries } from '../api/entries';
import { getLanguage } from '../i18n/lang';
import { useStore } from '../state/store';
import { Status } from '../types';

export function useAutoFetch(): void {
  const groups = useStore((s) => s.groups);
  const markFetchStart = useStore((s) => s.markFetchStart);
  const markFetchResult = useStore((s) => s.markFetchResult);

  useEffect(() => {
    const language = getLanguage();
    for (const group of groups) {
      if (group.isLoading) continue;
      if (group.value === group.lastValue && group.category === group.lastCategory) continue;

      if (!group.value) {
        markFetchResult(group.id, '', group.category, {
          status: Status.Success,
          entries: null,
          matchCount: 0,
        });
        continue;
      }

      const { id, value, category } = group;
      markFetchStart(id);
      fetchEntries(value, category, language)
        .then((result) => markFetchResult(id, value, category, result))
        .catch(() =>
          markFetchResult(id, value, category, {
            status: Status.Failure,
            entries: null,
            matchCount: 0,
          }),
        );
    }
  }, [groups, markFetchStart, markFetchResult]);
}
