import * as E from 'fp-ts/Either';
import { pipe } from 'fp-ts/function';
import fs from 'fs';
import * as t from 'io-ts';
import puppeteer, { Browser, Page } from 'puppeteer-core';

const saveDataRT = t.type({
    executablePath: t.string,
    ouj: t.type({
        username: t.string,
        password: t.string,
    }),
});

type SaveData = t.TypeOf<typeof saveDataRT>;  

export const vod = async (configFilePath: string) => {
    if (!fs.existsSync(configFilePath)) {
        console.log('Configuration file not found');
        console.log(`In order to generate one, please execute \`ouj init\`.`);
        process.exit(0);
    }

    const text = (() => {
        try {
            return String(fs.readFileSync(configFilePath));
        } catch (e) {
            console.error('Failed to read configuration file');
            process.exit(1);
        }
    })();

    const parseResult = (() => {
        try {
            return JSON.parse(text);
        } catch (e) {
            console.error('Failed to parse configuration file');
            process.exit(1);
        }
    })();

    const saveData: SaveData =
        pipe(
            parseResult,
            saveDataRT.decode,
            E.getOrElseW(() => {
                console.error('Invalid configuration');
                process.exit(1);
            })
        );

    const launchBrowser = (): Promise<Browser> =>
        puppeteer.launch({
            headless: false,
            executablePath: saveData.executablePath,
            ignoreDefaultArgs: ['--mute-audio'],
            userDataDir: './userData',
        });

    const setupPage = async (browser: Browser): Promise<Page> => {
        const pages = await browser.pages();
        const page = (pages.length > 0) ? pages[0]! : await browser.newPage();
        await page.setUserAgent('Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100');
        return page;
    };

    const getCourseUrl = <T extends number>(courseId: T) =>
        `https://v.ouj.ac.jp/view/ouj/#/navi/player?co=${courseId}&ct=V` as const;

    const login = async (page: Page): Promise<void> => {
        await page.goto('https://v.ouj.ac.jp/view/ouj');

        const button = await page.waitForSelector('.login-button');
        await button!.click();
        await page.waitForNavigation({ waitUntil: 'networkidle0' });

        await page.type('input[name="username"]', saveData.ouj.username);
        await page.type('input[name="password"]', saveData.ouj.password);
        const submitButton = await page.waitForSelector('.btn-submit');
        await submitButton!.click();
        await page.waitForNavigation({ waitUntil: 'networkidle0' });

        console.log('Successfully logged in');
    };

    const browser = await launchBrowser();
    const page = await setupPage(browser);

    await login(page);

    await page.goto(getCourseUrl(8178));

    const playButton = await page.waitForSelector('.vjs-big-play-button');
    playButton!.click();
    console.log('Now playing...');
};
