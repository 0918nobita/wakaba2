import path from 'path';
import * as webpack from 'webpack';

const config: webpack.Configuration = {
    target: 'node',
    mode: 'development',
    entry: './src/OujCli.fsproj',
    output: {
        libraryTarget: 'commonjs',
        path: path.join(__dirname, './lib'),
        filename: 'index.js',
    },
    externals: [
        'puppeteer-core',
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
