import { checkArgument } from './errors';
import { IResponse, IEntry, Status, EntryType, EntryCategory } from './types';
import { antiForgeryName, antiForgeryValue, language } from './app.module';

export class Service {

    static $inject = ['$http', '$q'];

    constructor(private $http: ng.IHttpService, private $q: ng.IQService) {
    }

    getEntries(query: string, category: EntryCategory): ng.IPromise<IResponse> {
        checkArgument(query, 'query');

        const data = { query, category, language };
        data[antiForgeryName] = antiForgeryValue;

        const request = {
            method: 'POST',
            url: '/xhr/entries',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=utf-8' },
            data: this.formEncode(data)
        };

        return this.$http(request)
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

    private formEncode(data: {}): string {
        let encoded = '';

        for (let key in data) {

            if (encoded.length > 0)
                encoded += '&';

            encoded += key + '=' + encodeURIComponent(data[key]);
        }

        return encoded;
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
