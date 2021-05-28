chrome.browserAction.onClicked.addListener(() => {
    chrome.windows.create({
        focused: true,
        type: 'normal',
        url: 'portal.html',
    });
});
