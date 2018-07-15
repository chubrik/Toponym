import { checkArgument } from './errors';
import { GroupType, IResponse, IEntry, Status } from './types';
import { language } from './app.module';

export class Service {

    static $inject = ['$http', '$q'];

    constructor(private $http: ng.IHttpService, private $q: ng.IQService) {
    }

    getEntries(query: string, type: GroupType): ng.IPromise<IResponse> {
        checkArgument(query, 'query');

        return this.$http
            .post<IResponse>('/xhr/entries', { query, type, language })
            .then((callback: any) => {
                const response: IResponse = callback.data;

                if (!response || response.status === Status.Failure)
                    return this.$q.reject();

                if (response.entries)
                    for (let entry of response.entries)
                        this.initLocal(entry);

                return response;
            });
    }

    private initLocal(entry: IEntry): void {

        if (entry.type >= 100 && entry.type < 200)
            entry._typeClass = 'populated';

        else if (entry.type >= 200 && entry.type < 300)
            entry._typeClass = 'water';
    }
}
