import { RouteComponentProps } from 'react-router';

export class FetchUtils {
    public static get(url: string, callback: (json: any)=> void) : void {
        FetchUtils.fetchWithMethod(url, 'GET', callback);
    }

    public static post(url: string, callback: (json: any) => void): void {
        FetchUtils.fetchWithMethod(url, 'POST', callback);
    }

    private static fetchWithMethod(url: string, requestMethod: string, callback: (json: any) => void): void {
        fetch(url, {
            method: requestMethod
        })
        .then(function (response) {
            return response.json();
        })
        .then(function (json) {
            callback(json);
        })
        .catch(function (ex) {
            console.error('parsing failed', ex);
        });
    }

    public static repeatGet(
        url: string,
        callback: (json: any) => void,
        repeatCondition: (json: any) => boolean,
        repeatPeriod: number)
    {
        FetchUtils.repeatFetch(url, 'GET', callback, repeatCondition, repeatPeriod);
    }

    public static repeatPost(
        url: string,
        callback: (json: any) => void,
        repeatCondition: (json: any) => boolean,
        repeatPeriod: number)
    {
        FetchUtils.repeatFetch(url, 'POST', callback, repeatCondition, repeatPeriod);
    }

    private static repeatFetch(
        url: string,
        requestMethod: string,
        callback: (json: any) => void,
        repeatCondition: (json: any) => boolean,
        repeatPeriod: number): void
    {
        var doCallbackAndRepeatRequest = function (json: any): void {
            callback(json);
            if (repeatCondition(json))
                setTimeout(function () { FetchUtils.repeatFetch(url, requestMethod, callback, repeatCondition, repeatPeriod); }, repeatPeriod);
        };

        FetchUtils.fetchWithMethod(url, requestMethod, doCallbackAndRepeatRequest);
    }
}