import { Command, flags } from '@oclif/command';
import dotenv from 'dotenv';
import puppeteer, { Browser } from 'puppeteer-core';

const launchBrowser = () =>
  puppeteer.launch({
    headless: false,
    executablePath: process.env['CHROME_EXE_PATH'],
    ignoreDefaultArgs: ['--mute-audio'],
    userDataDir: './userData',
  });

const setupPage = async (browser: Browser) => {
  const pages = await browser.pages();
  const page = (pages.length > 0) ? pages[0]! : await browser.newPage();
  await page.setUserAgent('Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100');
  return page;
};

class OujCommand extends Command {
  static description = '放送大学のVODシステムを快適に利用するためのCLI';

  static flags = {
    version: flags.version({ char: 'v' }),
    help: flags.help({ char: 'h' }),
  };

  async run() {
    this.parse(OujCommand);

    dotenv.config();

    const browser = await launchBrowser();
    const page = await setupPage(browser);

    await page.goto('https://v.ouj.ac.jp/view/ouj');

    const button = await page.waitForSelector('.login-button');
    await button!.click();
    await page.waitForNavigation({ waitUntil: 'networkidle0' });

    await page.type('input[name="username"]', process.env['OUJ_USERNAME']!);
    await page.type('input[name="password"]', process.env['OUJ_PASSWORD']!);
    const submitButton = await page.waitForSelector('.btn-submit');
    await submitButton!.click();
    await page.waitForNavigation({ waitUntil: 'networkidle0' });
    console.log('Successfully logged in');

    await page.goto('https://v.ouj.ac.jp/view/ouj/#/navi/player?co=8178&ct=V&ca=930');

    const playButton = await page.waitForSelector('.vjs-big-play-button');
    playButton!.click();
    console.log('Now playing...');
  }
}

export = OujCommand;
