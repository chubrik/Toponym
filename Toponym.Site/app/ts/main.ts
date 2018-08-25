import { checkArgument, invalidOperation } from './errors';
import { IGroup, IEntry, Status, allEntryCategories, EntryCategory } from './types';
import { defaultHost, fbAppId } from './app.module';
import { langText, rusCase } from './utils';
import { Service } from './service';
import { UrlHelper } from './url.helper';

export class MainController {

    private loadDelay = 1000;
    groups: IGroup[] = [];
    currentGroupIndex = 0;
    entriesShowLimit: number | null = null;
    private expandedEntry: IEntry | null;
    private timer: ng.IPromise<void>;
    static $inject = ['$scope', '$timeout', '$q', 'service', 'url'];

    constructor(
        $scope: ng.IScope, private $timeout: ng.ITimeoutService, private $q: ng.IQService,
        private service: Service, private url: UrlHelper) {

        this.init();

        $scope.$on('navigate', () => {
            this.onNavigate();
        });

        this.onNavigate();
    }

    private init(): void {
        const queries = this.url.getQueries();

        if (!queries.length)
            queries.push({ value: '', category: allEntryCategories });

        for (let query of queries) {
            this.groups.push({
                value: query.value,
                category: query.category,
                status: Status.Success
            } as IGroup);
        }

        this.$timeout(() => {
            $('input[type=text]')[0].focus();
        });
    }

    private onNavigate(): void {

        for (let group of this.groups) {
            //console.log(`onNavigate: ${group.value}, last: ${group.lastValue}`);

            if (group.value === group.lastValue && group.category === group.lastCategory)
                continue;

            if (!group.value) {
                group.status = Status.Success;
                group.entries = null;
                group.lastValue = '';
                group.lastCategory = allEntryCategories;
                continue;
            }

            const value = group.value;
            const category = group.category;
            group.isLoading = group.isLoading || 0;
            group.isLoading++;

            this.service
                .getEntries(group.value, group.category)
                .then(response => {
                    group.lastValue = value;
                    group.lastCategory = category;
                    group.status = response.status;
                    group.entries = response.entries;
                    group.matchCount = response.matchCount;
                    this.entriesShowLimit = 50;
                    return this.$timeout();
                })
                .catch(() => {
                    group.status = Status.Failure;
                    group.entries = null;
                })
                .finally(() => {
                    this.entriesShowLimit = null;

                    if (group.isLoading)
                        group.isLoading--;
                });
        }
    }

    currentGroup(): IGroup {
        return this.groups[this.currentGroupIndex];
    }

    onAddGroup(): void {
        this.groups.push({
            value: '',
            category: this.groups[this.groups.length - 1].category,
            status: Status.Success
        } as IGroup);

        this.$timeout(() => {
            const index = this.groups.length - 1;
            $('input[type=text]')[index].focus();
        });
    }

    onDeleteGroup(index: number): void {
        checkArgument(index, 'index');

        const focusIndex = index < this.groups.length - 1 ? index + 1 : index - 1;
        $('input[type=text]')[focusIndex].focus();
        this.groups.splice(index, 1);
        this.url.go(this.groups);
    }

    onChangeGroupValue(group: IGroup): void {
        checkArgument(group, 'group');

        //console.log(`onChangeGroupValue: ${group.value}, last: ${group.lastValue}`);
        this.$timeout.cancel(this.timer);

        if (!group.value) {
            this.url.go(this.groups);
            return;
        }

        this.timer =
            this.$timeout(() => {
                this.url.go(this.groups);
            }, this.loadDelay);
    }

    onSelectGroup(index: number): ng.IPromise<void> {
        checkArgument(index, 'index');

        if (index === this.currentGroupIndex)
            return this.$q.when();

        $(window).scrollTop(0);
        const side = $('#side');
        const transition = side.css('transition');

        // ReSharper disable once RedundantUnits
        side.css({ transition: '0s' })
            .removeClass(`color${this.currentGroupIndex + 1}`)
            .addClass(`color${index + 1}`);

        this.currentGroupIndex = index;
        this.entriesShowLimit = 50;

        return this.$timeout()
            .then(() => {
                side.css({ transition: transition });
                this.entriesShowLimit = null;
                return this.$timeout();
            });
    }

    isSilentedGroup(index: number): boolean {
        checkArgument(index, 'index');

        return this.groups[index] &&
            !this.groups[index].isIntensive &&
            this.groups.some(i => i.isIntensive);
    }

    isOneAndEmpty(index: number): boolean {
        checkArgument(index, 'index');

        return index === 0 && this.groups.length === 1 && !this.groups[0].value;
    }

    onClickMark(entry: IEntry, groupIndex: number): void {
        checkArgument(entry, 'entry');
        checkArgument(groupIndex, 'groupIndex');

        const windowHeight = $(window).height();

        if (groupIndex === this.currentGroupIndex) {

            const entryElement = $('#side .entry.highlight');
            const entryElementOffset = entryElement.offset();

            if (!entryElement.length)
                throw invalidOperation();

            if (!entry._isExpanded)
                this.onToggleEntry(entry);

            if (entryElementOffset != null && windowHeight != null)
                // "html" для FireFox
                $('html, body').animate({
                    scrollTop: entryElementOffset.top - windowHeight / 2.5
                }, 500);
        }
        else {
            this.onSelectGroup(groupIndex)
                .then(() => {
                    const entryElement = $('#side .entry.highlight');
                    const entryElementOffset = entryElement.offset();

                    if (!entryElement.length)
                        throw invalidOperation();

                    if (!entry._isExpanded)
                        this.onToggleEntry(entry);

                    if (entryElementOffset != null && windowHeight != null)
                        // "html" для FireFox
                        $('html, body')
                            .scrollTop(entryElementOffset.top - windowHeight / 2.5);
                });
        }
    }

    onToggleEntry(entry: IEntry): void {
        checkArgument(entry, 'entry');

        if (!entry._isExpanded) {
            if (this.expandedEntry)
                this.expandedEntry._isExpanded = false;

            this.expandedEntry = entry;
        }
        else
            this.expandedEntry = null;

        entry._isExpanded = !entry._isExpanded;
    }

    shareFbUrl(): string {
        return 'https://www.facebook.com/dialog/share?display=popup&app_id=' + fbAppId +
            '&href=' + encodeURIComponent(this.canonicalUrl());
    }

    shareVkUrl(): string {
        return 'http://vk.com/share.php?url=' + encodeURIComponent(this.canonicalUrl());
    }

    shareTwUrl(): string {
        const firstQuery = this.groups[0].value;
        let text = langText(
            'Смотрите, что есть на карте Беларуси!',
            'Глядзіце, што ёсць на карце Беларусі!',
            'Look what is on the map of Belarus!');

        if (firstQuery) {
            const count = this.groups[0].matchCount || 0;

            const found = langText(
                rusCase(count, ['топоним', 'топонима', 'топонимов']),
                rusCase(count, ['тапонім', 'тапоніма', 'тапонімаў']),
                count + (count === 1 ? ' toponym' : ' toponyms')) + ` "${firstQuery}"`;

            const textRu =
                rusCase(count, ['Найден', 'Найдено', 'Найдено'], false /* includeNumber */) +
                ` ${found} на карте Беларуси.`;

            const textBe =
                rusCase(count, ['Знойдзены', 'Знойдзена', 'Знойдзена'], false /* includeNumber */) +
                ` ${found} на карце Беларусі.`;

            text = langText(textRu, textBe, `Found ${found} on the map Belarus.`);
        }

        const tags = langText(
            '#топоним #топонимика #беларусь #белоруссия',
            '#топоним #топонимика #беларусь #белоруссия',
            '#toponym #toponymy #belarus #belorussia');

        return 'https://twitter.com/intent/tweet?text=' +
            encodeURIComponent(text + '\n\n' + this.canonicalUrl() + '\n' + tags);
    }

    private canonicalUrl(): string {
        return 'http://' + defaultHost + window.location.pathname + window.location.search;
    }

    onClickShareButton($event: Event, suffix: string): void {
        checkArgument($event, '$event');
        checkArgument(suffix, 'suffix');

        $event.preventDefault();

        window.open(
            ($event.currentTarget as HTMLAnchorElement).href,
            'share_' + suffix, 'width=600, height=400');
    }

    onSetValue(value: string): void {
        checkArgument(value, 'value');

        this.groups[this.currentGroupIndex].value = value;
        this.url.go(this.groups);
        $('input[type=text]')[this.currentGroupIndex].focus();
    }

    onReset(): void {
        this.groups.splice(1);
        this.groups[0].value = '';
        this.groups[0].category = allEntryCategories;
        this.url.go(this.groups);
        $('input[type=text]')[0].focus();
    }

    isReseted(): boolean {
        return this.groups.length === 1 &&
            !this.groups[0].value &&
            this.groups[0].category === allEntryCategories;
    }

    isQueryEmpty(): boolean {
        const group = this.currentGroup();
        return group && group.status === Status.Success && !group.entries;
    }

    isNoEntriesFound(): boolean {
        const group = this.currentGroup();

        return group &&
            group.status === Status.Success &&
            !!group.entries &&
            !group.entries.length;
    }

    isQuerySyntaxError(): boolean {
        const group = this.currentGroup();
        return group && group.status === Status.SyntaxError;
    }

    isServerError(): boolean {
        const group = this.currentGroup();
        return group && group.status === Status.Failure;
    }

    isCuttedList(): boolean {
        const group = this.currentGroup();
        return !!group.entries && group.entries.length < group.matchCount;
    }

    isShowPopulated(): boolean {
        return Boolean(this.currentGroup().category & EntryCategory.Populated);
    }

    isShowWater(): boolean {
        return Boolean(this.currentGroup().category & EntryCategory.Water);
    }

    isShowLocality(): boolean {
        return Boolean(this.currentGroup().category & EntryCategory.Locality);
    }

    onClickShowPopulated(event: JQueryMouseEventObject): void {
        this.onClickShowCategory(event, EntryCategory.Populated);
    }

    onClickShowWater(event: JQueryMouseEventObject): void {
        this.onClickShowCategory(event, EntryCategory.Water);
    }

    onClickShowLocality(event: JQueryMouseEventObject): void {
        this.onClickShowCategory(event, EntryCategory.Locality);
    }

    private onClickShowCategory(event: JQueryMouseEventObject, category: EntryCategory): void {
        checkArgument(event, 'event');

        const isChecked = (event.currentTarget as HTMLInputElement).checked;
        const currentCategory = this.currentGroup().category;
        let newCategory = isChecked ? currentCategory + category : currentCategory - category;

        if (!newCategory)
            newCategory = allEntryCategories - category;

        this.currentGroup().category = newCategory;
        $(window).scrollTop(0);
        this.url.go(this.groups);
    }
}
