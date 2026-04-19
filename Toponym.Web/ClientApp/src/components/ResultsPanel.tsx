import { useEffect } from 'react';
import { useStore } from '../state/store';
import { langText, rusCase, getLanguage } from '../i18n/lang';
import { entryTypeAbbr, entryTypeText } from '../i18n/entryText';
import { linkOsm, linkGoogle, linkYandex, linkRetromap } from '../utils/entryLinks';
import { EntryCategory, Language, Status, allEntryCategories } from '../types';
import type { Entry, Group } from '../types';
import { Tutorial } from './Tutorial';

function countMessage(matchCount: number, cutted: boolean, shown: number): string {
  const lang = getLanguage();
  const suffix = cutted ? '.' : ':';
  if (lang === Language.English) {
    const word = matchCount === 1 ? 'toponym' : 'toponyms';
    const base = `Found ${matchCount} ${word}${suffix}`;
    return cutted ? `${base}\nThe first ${shown} of these:` : base;
  }
  if (lang === Language.Belarusian) {
    const verb = rusCase(matchCount, ['Знойдзен', 'Знойдзена'], false);
    const noun = rusCase(matchCount, ['тапонім', 'тапоніма', 'тапонімаў']);
    const base = `${verb} ${noun}${suffix}`;
    return cutted ? `${base}\nПершыя ${shown} з іх:` : base;
  }
  const verb = rusCase(matchCount, ['Найден', 'Найдено'], false);
  const noun = rusCase(matchCount, ['топоним', 'топонима', 'топонимов']);
  const base = `${verb} ${noun}${suffix}`;
  return cutted ? `${base}\nПервые ${shown} из них:` : base;
}

function CategoryFilters({ group }: { group: Group }) {
  const currentGroupIndex = useStore((s) => s.currentGroupIndex);
  const toggleCategory = useStore((s) => s.toggleCategory);
  const isOn = (cat: EntryCategory) => (group.category & cat) !== 0;

  return (
    <div className="group-categories">
      <label>
        <input
          type="checkbox"
          checked={isOn(EntryCategory.Populated)}
          onChange={() => toggleCategory(currentGroupIndex, EntryCategory.Populated)}
        />{' '}
        {langText('Ойконимы', 'Айконімы', 'Oikonyms')}
      </label>
      <label>
        <input
          type="checkbox"
          checked={isOn(EntryCategory.Water)}
          onChange={() => toggleCategory(currentGroupIndex, EntryCategory.Water)}
        />{' '}
        {langText('Гидронимы', 'Гідронімы', 'Hydronyms')}
      </label>
      <label>
        <input
          type="checkbox"
          checked={isOn(EntryCategory.Locality)}
          onChange={() => toggleCategory(currentGroupIndex, EntryCategory.Locality)}
        />{' '}
        {langText('Малые топонимы', 'Малыя тапонімы', 'Small toponyms')}
      </label>
    </div>
  );
}

function EntryItem({ entry }: { entry: Entry }) {
  const isHighlighted = useStore((s) => s.sideHighlightEntry === entry);
  const isExpanded = useStore((s) => s.expandedEntry === entry);
  const setMapHighlightEntry = useStore((s) => s.setMapHighlightEntry);
  const toggleExpandedEntry = useStore((s) => s.toggleExpandedEntry);
  const highlight = isHighlighted ? ' highlight' : '';
  const expanded = isExpanded ? ' expanded' : '';

  return (
    <div className={`entry${highlight}${expanded}`}>
      <span
        className="title"
        onMouseEnter={() => setMapHighlightEntry(entry)}
        onMouseLeave={() => setMapHighlightEntry(null)}
        onClick={() => toggleExpandedEntry(entry)}
      >
        {entry.title}
      </span>
      {!isExpanded && (
        <>
          {' '}
          <span className="type-abbr">{entryTypeAbbr(entry)}</span>
        </>
      )}
      <div>
        {entryTypeText(entry)}
        <br />
        {langText('На картах', 'На картах', 'On maps')}:{' '}
        <a href={linkOsm(entry)} target="_blank" tabIndex={-1} rel="noreferrer">O</a>,{' '}
        <a href={linkGoogle(entry)} target="_blank" tabIndex={-1} rel="noreferrer">G</a>,{' '}
        <a href={linkYandex(entry)} target="_blank" tabIndex={-1} rel="noreferrer">
          {langText('Я', 'Я', 'Y')}
        </a>,{' '}
        <a href={linkRetromap(entry)} target="_blank" tabIndex={-1} rel="noreferrer">R</a>
      </div>
    </div>
  );
}

export function ResultsPanel() {
  const groups = useStore((s) => s.groups);
  const currentGroupIndex = useStore((s) => s.currentGroupIndex);
  const entriesShowLimit = useStore((s) => s.entriesShowLimit);
  const scrollToTopToken = useStore((s) => s.scrollToTopToken);
  const scrollToExpandedToken = useStore((s) => s.scrollToExpandedToken);

  useEffect(() => {
    if (!scrollToTopToken) return;
    window.scrollTo(0, 0);
  }, [scrollToTopToken]);

  useEffect(() => {
    if (!scrollToExpandedToken) return;
    const el = document.querySelector('#side .entry.expanded');
    if (el) el.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }, [scrollToExpandedToken]);

  const group = groups[currentGroupIndex];
  if (!group) return null;

  const hasResults = !!group.entries && group.entries.length > 0;
  const isEmpty =
    !group.entries &&
    group.status !== Status.SyntaxError &&
    group.status !== Status.Failure;
  const isNoEntries = group.status === Status.Success && !!group.entries && group.entries.length === 0;
  const isSyntaxError = group.status === Status.SyntaxError;
  const isServerError = group.status === Status.Failure;
  const cutted = hasResults && (group.entries?.length ?? 0) < group.matchCount;

  return (
    <div id="side" className={`color${currentGroupIndex + 1}`}>
      {(hasResults || isNoEntries) && <CategoryFilters group={group} />}
      {isEmpty && <Tutorial />}
      {isSyntaxError && (
        <p>{langText('Ошибочный запрос', 'Памылковы запыт', 'Bad request')}</p>
      )}
      {isServerError && (
        <p>
          {langText(
            'Ошибка связи с сервером',
            'Памылка сувязі з серверам',
            'Server connection error',
          )}
        </p>
      )}
      {isNoEntries && (
        <p className="group-categories-margin">
          {langText('Совпадений не найдено', 'Супадзенняў ня знойдзена', 'No matches found')}
        </p>
      )}
      {hasResults && (
        <p className="group-categories-margin" style={{ whiteSpace: 'pre-line' }}>
          {countMessage(group.matchCount, cutted, group.entries?.length ?? 0)}
        </p>
      )}
      {hasResults && (entriesShowLimit ? group.entries!.slice(0, entriesShowLimit) : group.entries!).map((entry, i) => (
        <EntryItem key={`${entry.title}-${i}`} entry={entry} />
      ))}
    </div>
  );
}

export { allEntryCategories };
