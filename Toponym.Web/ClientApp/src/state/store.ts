import { create } from 'zustand';
import { allEntryCategories, EntryCategory, Status } from '../types';
import type { Entry, Group } from '../types';

let nextId = 1;
const newId = () => nextId++;

export function createGroup(value = '', category = allEntryCategories): Group {
  return {
    id: newId(),
    value,
    category,
    lastValue: '',
    lastCategory: allEntryCategories,
    status: null,
    entries: null,
    matchCount: 0,
    isLoading: false,
  };
}

function initialGroupsFromUrl(): Group[] {
  const params = new URLSearchParams(window.location.search);
  const result: Group[] = [];
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
    result.unshift(createGroup(value ?? '', category));
  }
  return result.length ? result : [createGroup()];
}

const initialGroups = initialGroupsFromUrl();

interface SearchResult {
  status: Status;
  entries: Entry[] | null;
  matchCount: number;
}

interface StoreState {
  groups: Group[];
  currentGroupIndex: number;
  mapHighlightEntry: Entry | null;
  sideHighlightEntry: Entry | null;
  entriesShowLimit: number | null;
  expandedEntry: Entry | null;
  scrollToTopToken: number;
  scrollToExpandedToken: number;
  setGroups(groups: Group[]): void;
  setCurrentGroupIndex(index: number): void;
  addGroup(): void;
  deleteGroup(index: number): void;
  setGroupValue(index: number, value: string): void;
  toggleCategory(index: number, category: EntryCategory): void;
  reset(): void;
  setMapHighlightEntry(entry: Entry | null): void;
  setSideHighlightEntry(entry: Entry | null): void;
  toggleExpandedEntry(entry: Entry): void;
  setExpandedEntry(entry: Entry | null): void;
  expandAndScroll(entry: Entry, groupIndex: number): void;
  markFetchStart(id: number): void;
  markFetchResult(id: number, value: string, category: EntryCategory, result: SearchResult): void;
}

export const useStore = create<StoreState>((set) => ({
  groups: initialGroups,
  currentGroupIndex: 0,
  entriesShowLimit: null,
  mapHighlightEntry: null,
  sideHighlightEntry: null,
  expandedEntry: null,
  scrollToTopToken: 0,
  scrollToExpandedToken: 0,

  setMapHighlightEntry: (entry) => set({ mapHighlightEntry: entry }),
  setSideHighlightEntry: (entry) => set({ sideHighlightEntry: entry }),
  setExpandedEntry: (entry) => set({ expandedEntry: entry }),
  toggleExpandedEntry: (entry) =>
    set((state) => ({ expandedEntry: state.expandedEntry === entry ? null : entry })),
  expandAndScroll: (entry, groupIndex) => {
    const switchingGroup = groupIndex !== useStore.getState().currentGroupIndex;
    set((state) => ({
      currentGroupIndex: switchingGroup ? groupIndex : state.currentGroupIndex,
      expandedEntry: entry,
      entriesShowLimit: switchingGroup ? 50 : state.entriesShowLimit,
      scrollToTopToken: switchingGroup ? state.scrollToTopToken + 1 : state.scrollToTopToken,
      scrollToExpandedToken: switchingGroup ? state.scrollToExpandedToken : state.scrollToExpandedToken + 1,
    }));
    if (switchingGroup) {
      setTimeout(() => set((state) => ({
        entriesShowLimit: null,
        scrollToExpandedToken: state.scrollToExpandedToken + 1,
      })));
    }
  },

  setGroups: (groups) =>
    set((state) => ({
      groups,
      currentGroupIndex: Math.min(state.currentGroupIndex, Math.max(0, groups.length - 1)),
    })),

  setCurrentGroupIndex: (index) => {
    set({ currentGroupIndex: index, entriesShowLimit: 50 });
    setTimeout(() => set({ entriesShowLimit: null }));
  },

  addGroup: () =>
    set((state) => {
      if (state.groups.length >= 5) return state;
      const last = state.groups[state.groups.length - 1];
      return {
        groups: [...state.groups, createGroup('', last?.category ?? allEntryCategories)],
        currentGroupIndex: state.groups.length,
      };
    }),

  deleteGroup: (index) =>
    set((state) => {
      if (state.groups.length <= 1) return state;
      const groups = state.groups.filter((_, i) => i !== index);
      const currentGroupIndex = Math.min(state.currentGroupIndex, groups.length - 1);
      return { groups, currentGroupIndex };
    }),

  setGroupValue: (index, value) =>
    set((state) => {
      const groups = state.groups.map((g, i) => (i === index ? { ...g, value } : g));
      return { groups };
    }),

  toggleCategory: (index, category) =>
    set((state) => {
      const groups = state.groups.map((g, i) => {
        if (i !== index) return g;
        const isActive = (g.category & category) !== 0;
        let next = isActive ? g.category & ~category : g.category | category;
        if (!next) next = allEntryCategories & ~category;
        return { ...g, category: next };
      });
      return { groups };
    }),

  reset: () =>
    set({
      groups: [createGroup()],
      currentGroupIndex: 0,
    }),

  markFetchStart: (id) =>
    set((state) => ({
      groups: state.groups.map((g) => (g.id === id ? { ...g, isLoading: true } : g)),
    })),

  markFetchResult: (id, value, category, result) => {
    set((state) => ({
      groups: state.groups.map((g) =>
        g.id === id
          ? {
              ...g,
              isLoading: false,
              lastValue: value,
              lastCategory: category,
              status: result.status,
              entries: result.entries,
              matchCount: result.matchCount,
            }
          : g,
      ),
      entriesShowLimit: 50,
    }));
    setTimeout(() => set({ entriesShowLimit: null }));
  },
}));
