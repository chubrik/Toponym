import { checkArgument } from './errors';
import { IResponse, IEntry, Status, EntryType, EntryCategory } from './types';
import { language } from './app.module';

export class Service {

    static $inject = ['$http', '$q'];

    constructor(private $http: ng.IHttpService, private $q: ng.IQService) {
    }

    getEntries(query: string, category: EntryCategory): ng.IPromise<IResponse> {
        checkArgument(query, 'query');

        return this.$http
            .post<IResponse>('/xhr/entries', { query, category, language })
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
        const type = entry.type;

        if (type >= EntryType.Populated && type < EntryType.Water)
            entry._categoryClass = 'populated';

        else if (type >= EntryType.Water && type < EntryType.Locality)
            entry._categoryClass = 'water';

        else if (type >= EntryType.Locality)
            entry._categoryClass = 'locality';
    }
}
