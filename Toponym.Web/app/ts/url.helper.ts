import { checkArgument } from './errors';
import { IGroup, EntryCategory, allEntryCategories } from './types';

interface IStateParams {
    q1?: string;
    q2?: string;
    q3?: string;
    q4?: string;
    q5?: string;
    t1?: number;
    t2?: number;
    t3?: number;
    t4?: number;
    t5?: number;
}

export class UrlHelper {

    static $inject = ['$state', '$rootScope'];

    constructor(private $state: ng.ui.IStateService, $rootScope: ng.IRootScopeService) {

        $rootScope.$on('$locationChangeSuccess', () => {
            $rootScope.$broadcast('navigate');
        });
    }

    private getParams(): IStateParams {
        return this.$state.params as IStateParams;
    }

    getQueries(): { value: string; category: EntryCategory }[] {

        const params = this.getParams();
        const result: { value: string; category: EntryCategory }[] = [];

        let found = false;

        for (let i = 5; i > 0; i--) {
            const value = params['q' + i] as string;

            if (!value && !found)
                continue;

            found = true;
            let category = +(params['t' + i] as string);

            if (!category || category <= 0 || category > allEntryCategories)
                category = allEntryCategories;

            result.unshift({ value: value || '', category: category });
        }

        return result;
    }

    go(groups: IGroup[]): void {
        checkArgument(groups, 'groups');

        const params: IStateParams = {};

        for (let i = 0; i < 5; i++) {
            const group = groups[i];

            const showCategoryOnUrl =
                group && group.category && group.category != allEntryCategories;

            params['q' + (i + 1)] = group ? group.value : null;
            params['t' + (i + 1)] = showCategoryOnUrl ? group.category : null;
        }

        this.$state.go('app.queries', params);
    }
}
