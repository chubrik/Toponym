import { entryCategory, EntryCategory, EntryType } from '../types';
import type { Entry, Group } from '../types';
import { entryTypeAbbr } from '../i18n/entryText';
import { isPolyline, polylinePoints } from '../utils/entryLinks';
import { useStore } from '../state/store';
import { Tooltip } from './Tooltip';

function markClass(type: EntryType): string {
  switch (entryCategory(type)) {
    case EntryCategory.Populated: return 'populated';
    case EntryCategory.Water: return 'water';
    case EntryCategory.Locality: return 'locality';
    default: return '';
  }
}

function pointEntries(entries: Entry[] | null): Entry[] {
  return entries ? entries.filter((e) => !isPolyline(e)) : [];
}

function polylineEntries(entries: Entry[] | null): Entry[] {
  return entries ? entries.filter(isPolyline) : [];
}

function MarkItem({ entry, groupIndex }: { entry: Entry; groupIndex: number }) {
  const isHighlighted = useStore((s) => s.mapHighlightEntry === entry);
  const setSideHighlightEntry = useStore((s) => s.setSideHighlightEntry);
  const expandAndScroll = useStore((s) => s.expandAndScroll);

  return (
    <Tooltip label={`${entryTypeAbbr(entry)} ${entry.title}`}>
      <div
        className={`mark ${markClass(entry.type)}${isHighlighted ? ' highlight' : ''}`}
        style={{
          left: `${entry.screen[0][0]}%`,
          top: `${entry.screen[0][1]}%`,
        }}
        onMouseEnter={() => setSideHighlightEntry(entry)}
        onMouseLeave={() => setSideHighlightEntry(null)}
        onClick={() => expandAndScroll(entry, groupIndex)}
      />
    </Tooltip>
  );
}

function PolylineItem({ entry, groupIndex }: { entry: Entry; groupIndex: number }) {
  const isHighlighted = useStore((s) => s.mapHighlightEntry === entry);
  const setSideHighlightEntry = useStore((s) => s.setSideHighlightEntry);
  const expandAndScroll = useStore((s) => s.expandAndScroll);

  return (
    <Tooltip label={`${entryTypeAbbr(entry)} ${entry.title}`}>
      <polyline
        className={isHighlighted ? 'highlight' : ''}
        points={polylinePoints(entry)}
        onMouseEnter={() => setSideHighlightEntry(entry)}
        onMouseLeave={() => setSideHighlightEntry(null)}
        onClick={() => expandAndScroll(entry, groupIndex)}
      />
    </Tooltip>
  );
}

function GroupPoints({
  group,
  groupIndex,
  colorClass,
}: {
  group: Group;
  groupIndex: number;
  colorClass: string;
}) {
  return (
    <div className={`group ${colorClass}`}>
      {pointEntries(group.entries).map((entry, i) => (
        <MarkItem key={`${entry.title}-${i}`} entry={entry} groupIndex={groupIndex} />
      ))}
    </div>
  );
}

function GroupPolylines({
  group,
  groupIndex,
  colorClass,
}: {
  group: Group;
  groupIndex: number;
  colorClass: string;
}) {
  return (
    <g className={`group ${colorClass}`}>
      {polylineEntries(group.entries).map((entry, i) => (
        <PolylineItem key={`${entry.title}-${i}`} entry={entry} groupIndex={groupIndex} />
      ))}
    </g>
  );
}

export function SearchMap() {
  const groups = useStore((s) => s.groups);

  return (
    <div id="map">
      {groups.map((group, i) => (
        <GroupPoints key={group.id} group={group} groupIndex={i} colorClass={`color${i + 1}`} />
      ))}
      <svg width="100%" height="100%" viewBox="0 0 100 100">
        {groups.map((group, i) => (
          <GroupPolylines key={group.id} group={group} groupIndex={i} colorClass={`color${i + 1}`} />
        ))}
      </svg>
    </div>
  );
}
