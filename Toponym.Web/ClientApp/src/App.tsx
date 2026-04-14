import type { InitialState } from './initialState';
import { useAutoFetch } from './hooks/useAutoFetch';
import { useUrlState } from './hooks/useUrlState';
import { SearchForm } from './components/SearchForm';
import { SearchMap } from './components/SearchMap';
import { ResultsPanel } from './components/ResultsPanel';
import { ShareButtons } from './components/ShareButtons';

export function App({ initial }: { initial: InitialState }) {
  useUrlState();
  useAutoFetch();

  return (
    <>
      <SearchForm />
      <ResultsPanel />
      <SearchMap />
      <ShareButtons fbAppId={initial.fbAppId} defaultHost={initial.defaultHost} />
    </>
  );
}
