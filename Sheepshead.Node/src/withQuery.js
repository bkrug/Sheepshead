function getParamAccordingToType(key, queryParams) {
    var paramValue = queryParams[key];
    if (Array.isArray(paramValue))
        return paramValue.map(v => encodeURIComponent(key) + "=" + encodeURIComponent(v)).join('&');
    else if (typeof paramValue == "object")
        return encodeURIComponent(key) + "=" + encodeURIComponent(JSON.stringify(paramValue));
    else
        return encodeURIComponent(key) + "=" + encodeURIComponent(paramValue);
}

//Takes a url string and an object and appends to object to the url as query parameters.
export function withQuery(urlString, queryParams) {
    var pairs = Object.keys(queryParams)
          .map(key => getParamAccordingToType(key, queryParams));
    var firstSeperator = urlString.indexOf('?') >= 0 ? '&' : '?';
    return urlString + firstSeperator + pairs.join('&');
};