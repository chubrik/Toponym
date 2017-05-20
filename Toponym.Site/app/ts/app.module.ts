import { Language } from './types';
import { MainController } from './main';
import {
    rusCase, itemTypeAbbr, itemTypeText, linkLoadmap, linkOsm, linkGoogle, linkYandex,
    pointItems, polylineItems, polylinePoints
} from './utils';
import { Service } from './service';
import { UrlHelper } from './url.helper';

declare const angular: any;
export let defaultHost: string;
export let language: Language;
export let fbAppId: string;

export function startup(options: { defaultHost: string, language: Language, fbAppId: string })
    : void {

    defaultHost = options.defaultHost;
    language = options.language;
    fbAppId = options.fbAppId;
}

angular
    .module('toponym', ['ui.router', 'ui.bootstrap'])
    .service('service', Service)
    .service('url', UrlHelper)
    .config([
        '$logProvider', '$urlRouterProvider', '$locationProvider',
        ($logProvider: ng.ILogProvider, $urlRouterProvider: ng.ui.IUrlRouterProvider,
         $locationProvider: ng.ILocationProvider) => {
            $logProvider.debugEnabled(true);
            $urlRouterProvider.otherwise('/');
            $locationProvider.html5Mode(true);
        }
    ])
    .config([
        '$stateProvider', ($stateProvider: ng.ui.IStateProvider) => {
            $stateProvider
                .state({
                    name: 'app',
                    templateUrl: 'main',
                    controller: MainController,
                    controllerAs: 'ctrl'
                })
                .state({
                    name: 'app.queries',
                    url: '/?{q1:any}&:t1&{q2:any}&:t2&{q3:any}&:t3&{q4:any}&:t4&{q5:any}&:t5'
                });
        }
    ])
    .run([
        '$rootScope', ($rootScope: any) => {
            $rootScope.Core = {
                rusCase,
                itemTypeAbbr,
                itemTypeText,
                linkLoadmap,
                linkOsm,
                linkGoogle,
                linkYandex,
                pointItems,
                polylineItems,
                polylinePoints
            };
        }
    ]);
