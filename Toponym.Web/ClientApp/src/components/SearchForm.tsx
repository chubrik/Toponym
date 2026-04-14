import { useEffect, useRef, useState } from 'react';
import { langText } from '../i18n/lang';
import { useDebouncedCallback } from '../hooks/useDebouncedCallback';
import { useStore } from '../state/store';
import { allEntryCategories } from '../types';
import { Tooltip } from './Tooltip';

export function SearchForm() {
  const groups = useStore((s) => s.groups);
  const addGroup = useStore((s) => s.addGroup);
  const deleteGroup = useStore((s) => s.deleteGroup);
  const setGroupValue = useStore((s) => s.setGroupValue);
  const setCurrentGroupIndex = useStore((s) => s.setCurrentGroupIndex);
  const reset = useStore((s) => s.reset);

  const inputRefs = useRef<(HTMLInputElement | null)[]>([]);
  const [pending, setPending] = useState<{ index: number; value: string } | null>(null);

  const commit = useDebouncedCallback((index: number, value: string) => {
    setPending((p) => (p?.index === index ? null : p));
    setGroupValue(index, value);
  }, 1000);

  const handleChange = (index: number, value: string) => {
    setPending({ index, value });
    if (!value) {
      commit.cancel();
      setGroupValue(index, '');
    } else {
      commit.invoke(index, value);
    }
  };

  const handleAddGroup = () => {
    const newIndex = groups.length;
    commit.cancel();
    setPending(null);
    addGroup();
    requestAnimationFrame(() => inputRefs.current[newIndex]?.focus());
  };

  const handleDeleteGroup = (index: number) => {
    commit.cancel();
    setPending(null);
    deleteGroup(index);
    const focusIndex = Math.max(0, index - 1);
    requestAnimationFrame(() => inputRefs.current[focusIndex]?.focus());
  };

  useEffect(() => {
    inputRefs.current[0]?.focus();
  }, []);

  useEffect(() => {
    inputRefs.current = inputRefs.current.slice(0, groups.length);
  }, [groups.length]);

  const isReset =
    groups.length === 1 &&
    !groups[0].value &&
    groups[0].category === allEntryCategories;

  const placeholder = langText(
    'введите часть названия',
    'увядзіце частку назвы',
    'type part of title',
  );

  return (
    <div id="form">
      {groups.map((group, index) => {
        const colorClass = `color${index + 1}`;
        const displayValue =
          pending?.index === index ? pending.value : group.value;
        return (
          <div key={group.id} className="input-group hover-area">
            <input
              ref={(el) => {
                inputRefs.current[index] = el;
              }}
              className={`${colorClass}${groups.length === 1 && !group.value ? ' with-background' : ''}`}
              type="text"
              value={displayValue}
              spellCheck={false}
              placeholder={placeholder}
              onChange={(e) => handleChange(index, e.target.value)}
              onFocus={() => setCurrentGroupIndex(index)}
            />
            {group.isLoading && (
              <a className={`loading ${colorClass}`} tabIndex={-1}>
                <i className="mi mi-process mi-lg" />
              </a>
            )}
            {groups.length > 1 && !group.isLoading && (
              <div className="delete-group hover">
                <Tooltip label={langText('удалить', 'выдаліць', 'delete')} placement="right">
                  <a
                    className={colorClass}
                    href=""
                    tabIndex={-1}
                    onClick={(e) => {
                      e.preventDefault();
                      handleDeleteGroup(index);
                    }}
                  >
                    <i className="mi mi-x mi-lg" />
                  </a>
                </Tooltip>
              </div>
            )}
          </div>
        );
      })}
      {groups.length < 6 && (
        <div className="add-group">
          {groups.length < 5 && (
            <a
              className={`underline-area color${groups.length + 1}`}
              href=""
              tabIndex={-1}
              onClick={(e) => {
                e.preventDefault();
                handleAddGroup();
              }}
            >
              <i className="mi mi-plus" />{' '}
              <span className="underline">
                {langText('добавить запрос', 'дадаць запыт', 'add query')}
              </span>
            </a>
          )}
          {!isReset && (
            <a
              className={`reset${groups.length === 5 ? ' near' : ''}`}
              href=""
              tabIndex={-1}
              onClick={(e) => {
                e.preventDefault();
                commit.cancel();
                setPending(null);
                reset();
                requestAnimationFrame(() => inputRefs.current[0]?.focus());
              }}
            >
              {langText('сбросить', 'скінуць', 'reset')}
            </a>
          )}
        </div>
      )}
    </div>
  );
}
