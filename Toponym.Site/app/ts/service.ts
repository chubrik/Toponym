import { checkArgument } from './errors';
import { GroupType, IResponse, IItem, Status } from './types';
import { language } from './app.module';

export class Service {

    static $inject = ['$http', '$q'];

    constructor(private $http: ng.IHttpService, private $q: ng.IQService) {
    }

    getItems(query: string, type: GroupType): ng.IPromise<IResponse> {
        checkArgument(query, 'query');

        return this.$http
            .post<IResponse>('/ajax/items', { query, type, language })
            .then((callback: any) => {
                const response: IResponse = callback.data;

                if (!response || response.status === Status.Failure)
                    return this.$q.reject();

                if (response.items)
                    for (let item of response.items)
                        this.initLocal(item);

                return response;
            });
    }

    private initLocal(item: IItem): void {

        if (item.type >= 100 && item.type < 200)
            item._typeClass = 'populated';

        else if (item.type >= 200 && item.type < 300)
            item._typeClass = 'water';
    }
}
