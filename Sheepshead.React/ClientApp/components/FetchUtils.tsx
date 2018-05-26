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
}