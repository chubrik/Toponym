import { checkArgument, invalidOperation } from './errors';
import { IGroup, IItem, GroupType, Status } from './types';
import { defaultHost, fbAppId } from './app.module';
import { langText, rusCase } from './utils';
import { Service } from './service';
import { UrlHelper } from './url.helper';

export class MainController {

    private loadDelay = 1000;
    groups: IGroup[] = [];
    currentGroupIndex = 0;
    itemsShowLimit: number | null = null;
    private expandedItem: IItem | null;
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
            queries.push({ value: '', type: GroupType.All });

        for (let query of queries) {
            this.groups.push({
                value: query.value,
                type: query.type,
                status: Status.Success
            } as IGroup);
        }

        this.$timeout(() => {
            $('input')[0].focus();
        });
    }

    private onNavigate(): void {

        for (let group of this.groups) {
            //console.log(`onNavigate: ${group.value}, last: ${group.lastValue}`);

            if (group.value === group.lastValue && group.type === group.lastType)
                continue;

            if (!group.value) {
                group.status = Status.Success;
                group.items = null;
                group.lastValue = '';
                group.lastType = GroupType.All;
                continue;
            }

            const value = group.value;
            const type = group.type;
            group.isLoading = group.isLoading || 0;
            group.isLoading++;

            this.service
                .getItems(group.value, group.type)
                .then(response => {
                    group.lastValue = value;
                    group.lastType = type;
                    group.status = response.status;
                    group.items = response.items;
                    group.matchCount = response.matchCount;
                    this.itemsShowLimit = 50;
                    return this.$timeout();
                })
                .catch(() => {
                    group.status = Status.Failure;
                    group.items = null;
                })
                .finally(() => {
                    this.itemsShowLimit = null;

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
            type: this.groups[this.groups.length - 1].type,
            status: Status.Success
        } as IGroup);

        this.$timeout(() => {
            const index = this.groups.length - 1;
            $('input')[index].focus();
        });
    }

    onDeleteGroup(index: number): void {
        checkArgument(index, 'index');

        const focusIndex = index < this.groups.length - 1 ? index + 1 : index - 1;
        $('input')[focusIndex].focus();
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
        this.itemsShowLimit = 50;

        return this.$timeout()
            .then(() => {
                side.css({ transition: transition });
                this.itemsShowLimit = null;
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

    onClickMark(item: IItem, groupIndex: number): void {
        checkArgument(item, 'item');
        checkArgument(groupIndex, 'groupIndex');

        if (groupIndex === this.currentGroupIndex) {

            const itemElement = $('#side .item.highlight');

            if (!itemElement.length)
                throw invalidOperation();

            if (!item._isExpanded)
                this.onToggleItem(item);

            // "html" для FireFox
            $('html, body').animate({
                scrollTop: itemElement.offset().top - $(window).height() / 2.5
            }, 500);
        }
        else {
            this.onSelectGroup(groupIndex)
                .then(() => {
                    const itemElement = $('#side .item.highlight');

                    if (!itemElement.length)
                        throw invalidOperation();

                    if (!item._isExpanded)
                        this.onToggleItem(item);

                    // "html" для FireFox
                    $('html, body')
                        .scrollTop(itemElement.offset().top - $(window).height() / 2.5);
                });
        }
    }

    onToggleItem(item: IItem): void {
        checkArgument(item, 'item');

        if (!item._isExpanded) {
            if (this.expandedItem)
                this.expandedItem._isExpanded = false;

            this.expandedItem = item;
        }
        else
            this.expandedItem = null;

        item._isExpanded = !item._isExpanded;
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
        $('input')[this.currentGroupIndex].focus();
    }

    onReset(): void {
        this.groups.splice(1);
        this.groups[0].value = '';
        this.groups[0].type = GroupType.All;
        this.url.go(this.groups);
        $('input')[0].focus();
    }

    isReseted(): boolean {
        return this.groups.length === 1 &&
            !this.groups[0].value &&
            this.groups[0].type === GroupType.All;
    }

    isQueryEmpty(): boolean {
        const group = this.currentGroup();
        return group && group.status === Status.Success && !group.items;
    }

    isNoItemsFound(): boolean {
        const group = this.currentGroup();
        return group && group.status === Status.Success && !!group.items && !group.items.length;
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
        return !!group.items && group.items.length < group.matchCount;
    }

    isShowPopulated(): boolean {
        const group = this.currentGroup();
        return group.type === GroupType.All || group.type === GroupType.Populated;
    }

    isShowWater(): boolean {
        const group = this.currentGroup();
        return group.type === GroupType.All || group.type === GroupType.Water;
    }

    onClickShowPopulated(event: JQueryMouseEventObject): void {
        checkArgument(event, 'event');

        const show = (event.currentTarget as HTMLInputElement).checked;
        this.currentGroup().type = show ? GroupType.All : GroupType.Water;
        $(window).scrollTop(0);
        this.url.go(this.groups);
    }

    onClickShowWater(event: JQueryMouseEventObject): void {
        checkArgument(event, 'event');

        const show = (event.currentTarget as HTMLInputElement).checked;
        this.currentGroup().type = show ? GroupType.All : GroupType.Populated;
        $(window).scrollTop(0);
        this.url.go(this.groups);
    }
}
