import { checkArgument } from './errors';
import { GroupType, IGroup } from './types';

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

        $rootScope.$on('$stateChangeSuccess', () => {
            $rootScope.$broadcast('navigate');
        });
    }

    private getParams(): IStateParams {
        return this.$state.params as IStateParams;
    }

    getQueries(): { value: string; type: GroupType }[] {

        const params = this.getParams();
        const result: { value: string; type: GroupType }[] = [];

        let found = false;

        for (let i = 5; i > 0; i--) {
            const value = params['q' + i] as string;
            const type = params['t' + i] as string;

            if (!value && !found)
                continue;

            found = true;
            result.unshift({ value: value || '', type: +type || GroupType.All });
        }

        return result;
    }

    go(groups: IGroup[]): void {
        checkArgument(groups, 'groups');

        const params: IStateParams = {};

        for (let i = 0; i < 5; i++) {
            const group = groups[i];
            params['q' + (i + 1)] = group ? group.value : null;
            params['t' + (i + 1)] = group && group.type ? group.type : null;
        }

        this.$state.go('app.queries', params);
    }
}
