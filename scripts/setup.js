const CDP = require('chrome-remote-interface');

const sleep = (duration) =>
    new Promise((resolve) =>
        setTimeout(() => resolve(), duration));

const attemptCreatingCDPClient = async () => {
    for (let numRetries = 0; numRetries < 100; numRetries++) {
        try {
            const client = await CDP();
            console.info('Succeeded in creating CDP client!');
            return client;
        } catch {
            console.info('Failed to create CDP Client. Retrying...');
            await sleep(Math.pow(2, numRetries) * 100);
        }
    }
    throw new Error('Repeatedly failed to create CDP Client.');
};

(async () => {
    try {
        const client = await attemptCreatingCDPClient();
        const { Network, Page } = client;
        await Network.setUserAgentOverride({ userAgent: 'Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36' });
        await Network.enable();
        await Page.enable();
        await Page.navigate({ url: 'https://v.ouj.ac.jp/view/ouj/#/login' });
        await Page.loadEventFired();
        console.log('Complete.');
    } catch (e) {
        console.error(e);
    }
})();
