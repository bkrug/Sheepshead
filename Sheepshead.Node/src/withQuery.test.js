import { withQuery } from './withQuery';

test('withQuery - url has string parameters', () => {
    var expectedUrl = "http://mywebapi.com/home/get?param1=cat&param2=doggy";
    var actualUrl = withQuery("http://mywebapi.com/home/get", {
        param1: 'cat',
        param2: 'doggy'
    }).toString();

    expect(actualUrl).toEqual(expectedUrl);
});

test('withQuery - url has string and number parameters', () => {
    var expectedUrl = "http://mywebapi.com/home/get?param1=cat&smallNo=4782.15&param2=doggy";
    var actualUrl = withQuery("http://mywebapi.com/home/get", {
        param1: 'cat',
        smallNo: 4782.15,
        param2: 'doggy'
    }).toString();

    expect(actualUrl).toEqual(expectedUrl);
});

test('withQuery - url has array', () => {
    var expectedUrl = "https://catindiapers.com/home/get?requestVal=14&requestVal=-47&requestVal=2.71";
    var actualUrl = withQuery("https://catindiapers.com/home/get", {
        requestVal: [14, -47, 2.71]
    }).toString();

    expect(actualUrl).toEqual(expectedUrl);
});

test('withQuery - url has an object', () => {
    var someObj = {
        something: 12,
        otherthing: 'qp'
    };
    var serialized = encodeURIComponent(JSON.stringify(someObj));
    var expectedUrl = "https://school.edu/away/put?param3=abcde&param4=" + serialized;
    var actualUrl = withQuery("https://school.edu/away/put", {
        param3: 'abcde',
        param4: someObj
    }).toString();

    expect(actualUrl).toEqual(expectedUrl);
});