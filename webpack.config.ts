import path from 'path';
import * as webpack from 'webpack';

const config: webpack.Configuration = {
    mode: 'development',
    entry: './src/Wakaba2.fsproj',
    devtool: 'inline-source-map',
    output: {
        path: path.join(__dirname, './dist'),
        filename: 'contentScript.js',
    },
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
