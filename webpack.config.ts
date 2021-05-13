import path from 'path';
import * as webpack from 'webpack';

const config: webpack.Configuration = {
    target: 'node',
    mode: 'development',
    entry: './OujCli/OujCli.fsproj',
    output: {
        libraryTarget: 'commonjs',
        path: path.join(__dirname, './lib'),
        filename: 'index.js',
    },
    externals: [
        'puppeteer-core',
        'sqlite3',
    ],
    module: {
        rules: [
            {
                test: /\.fs(x|proj)?$/,
                loader: 'fable-loader',
            }
        ],
    },
};

export default config;
