/// <reference path="../node_modules/better-typescript-lib/lib.esnext.d.ts" />

import { init, vod } from './commands';
import { configFilePath } from './config';

const { version } = require('../package.json');

const { argv } = process;

const main = async () => {
    try {
        if (argv.length >= 3) {
            switch (argv[2]) {
                case 'init':
                    await init(configFilePath);
                    break;
                case 'vod':
                    await vod(configFilePath);
                    break;
                default:
                    throw new Error(`Unknown command: ${argv[2]}`);
            }
        } else {
            console.log(`OUJ CLI v${version}`);
        }
    } catch (e) {
        console.error(e);
        process.exit(1);
    }
};

void main();
