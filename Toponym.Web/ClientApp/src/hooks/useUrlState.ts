import { useEffect } from 'react';
import { useStore } from '../state/store';
import { buildSearch, groupsFromQueries, parseUrl } from '../state/url';

export function useUrlState(): void {
  const setGroups = useStore((s) => s.setGroups);
  const groups = useStore((s) => s.groups);

  useEffect(() => {
    const onPopState = () => {
      const queries = parseUrl(window.location.search);
      setGroups(groupsFromQueries(queries));
    };
    window.addEventListener('popstate', onPopState);
    return () => window.removeEventListener('popstate', onPopState);
  }, [setGroups]);

  useEffect(() => {
    const search = buildSearch(groups);
    if (search === window.location.search) return;
    const url = window.location.pathname + search + window.location.hash;
    window.history.replaceState(null, '', url);
  }, [groups]);
}
