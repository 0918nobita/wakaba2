chrome.webRequest.onBeforeSendHeaders.addListener((info) => {
    const headers =
        info.requestHeaders.filter(
            (header) => header.name !== 'User-Agent' && !(header.name.startsWith('sec-ch-ua')));
    const UA = 'Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36';
    headers.push({ name: 'User-Agent', value: UA });
    console.log({ info, headers })
    return { requestHeaders: headers };
}, {
    urls: ['https://v.ouj.ac.jp/*'],
    types: ['main_frame', 'sub_frame', 'stylesheet', 'script', 'image', 'font', 'object', 'xmlhttprequest', 'ping', 'csp_report', 'media', 'websocket', 'other'],
}, [
    'blocking',
    'requestHeaders',
]);

chrome.browserAction.onClicked.addListener(() => {
    chrome.windows.create({
        focused: true,
        type: 'normal',
        url: 'portal.html',
    });
});
