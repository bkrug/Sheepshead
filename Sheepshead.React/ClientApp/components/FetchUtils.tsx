import { RouteComponentProps } from 'react-router';

export class FetchUtils {
    public static get(url: string, callback: (json: any)=> void) : void {
        fetch(url, {
            method: 'GET'
        })
        .then(function (response) {
            return response.json();
        })
        .then(function (json) {
            callback(json);
        })
        .catch(function (ex) {
            console.log('parsing failed', ex)
        });
    }
}